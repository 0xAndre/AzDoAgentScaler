using AzDoAgentScaler.Services.Interfaces;
using AzDoAgentScaler.Utils;

namespace AzDoAgentScaler.Services;

/// <summary>
/// Orchestrates the scaling logic for Azure DevOps agents,
/// delegating API calls to IAzureDevOpsService and Docker operations to IDockerService.
/// </summary>
public class AgentScaler
{
    private readonly IAzureDevOpsService _azureDevOps;
    private readonly IDockerService _docker;
    private readonly ScalingOptions _options;
    private int _poolId;

    public AgentScaler(IAzureDevOpsService azureDevOps, IDockerService docker, ScalingOptions options)
    {
        _azureDevOps = azureDevOps;
        _docker = docker;
        _options = options;
    }

    /// <summary>
    /// Starts the scaling loop. Continuously monitors jobs and agents,
    /// and scales up or down as needed until cancellation is requested.
    /// </summary>
    /// <param name="token">Cancellation token to stop the loop.</param>
    public async Task RunAsync(CancellationToken token)
    {
        _poolId = await _azureDevOps.GetPoolIdByNameAsync(_options.PoolName)
                  ?? throw new InvalidOperationException($"Pool '{_options.PoolName}' not found.");

        ConsoleHelper.Info($"Pool '{_options.PoolName}' found with ID {_poolId}.");

        while (!token.IsCancellationRequested)
        {
            try
            {
                // Get the number of online agents and waiting jobs in the pool
                var onlineAgents = await _azureDevOps.GetOnlineAgentsAsync(_poolId);
                var waitingJobs = await _azureDevOps.GetWaitingJobsAsync(_poolId);

                ConsoleHelper.Info($"[{DateTime.UtcNow}] Online agents: {onlineAgents}, Waiting jobs: {waitingJobs}");

                // Ensure minimum number of agents are always running
                // This runs regardless of whether there are waiting jobs
                while (onlineAgents < _options.MinAgents)
                {
                    ConsoleHelper.Warn($"Online agents ({onlineAgents}) < MinAgents ({_options.MinAgents}). Creating agent...");
                    await ScaleUpAsync();
                    onlineAgents++;
                }

                // Scale up if there are waiting jobs and the number of agents is below the maximum
                if (waitingJobs > 0 && onlineAgents < _options.MaxAgents)
                {
                    ConsoleHelper.Warn($"Waiting jobs ({waitingJobs}) > 0 and online agents ({onlineAgents}) < MaxAgents ({_options.MaxAgents}). Scaling up...");
                    await ScaleUpAsync();
                    onlineAgents++;
                }

                // Scale down if there are no waiting jobs and the number of agents is above the minimum
                if (waitingJobs == 0 && onlineAgents > _options.MinAgents)
                {
                    ConsoleHelper.Warn("No waiting jobs and online agents > MinAgents. Scaling down...");
                    await ScaleDownAsync();
                }

                // No action needed if online agents are at minimum and no jobs are waiting
                if (waitingJobs == 0 && onlineAgents <= _options.MinAgents)
                {
                    ConsoleHelper.Debug("Minimum agents online, no waiting jobs. No action required.");
                }
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions during the scaling loop
                ConsoleHelper.Error($"Error during scaling loop: {ex.Message}");
            }

            // Wait for the configured interval before checking again
            await Task.Delay(_options.Interval * 1000, token);
        }

    }

    private async Task ScaleUpAsync()
    {
        var guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);
        var agentName = $"azdo-agent-{guidPart}";
        ConsoleHelper.Warn($"Scaling up: creating new agent '{agentName}'...");

        try
        {
            await _docker.CreateAgentAsync(
                _options.Organization,
                _options.PersonalAccessToken,
                _options.PoolName,
                agentName,
                _options.DockerImageName
            );

            ConsoleHelper.Success($"Agent '{agentName}' created successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Failed to create agent '{agentName}': {ex.Message}");
        }
    }

    private async Task ScaleDownAsync()
    {
        ConsoleHelper.Warn("Scaling down: looking for idle agent...");

        var idleAgent = await _azureDevOps.GetIdleAgentAsync(_poolId);
        if (idleAgent == null)
        {
            ConsoleHelper.Info("No idle agent found. Skipping scale down.");
            return;
        }

        try
        {
            var removedFromPool = await _azureDevOps.RemoveAgentFromPoolAsync(_poolId, idleAgent.Value.Id);
            if (!removedFromPool)
            {
                ConsoleHelper.Error($"Failed to remove agent '{idleAgent.Value.Name}' from Azure DevOps pool.");
                return;
            }

            await _docker.RemoveAgentAsync(idleAgent.Value.Name);
            ConsoleHelper.Success($"Agent '{idleAgent.Value.Name}' removed successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Failed to remove agent '{idleAgent.Value.Name}': {ex.Message}");
        }
    }
}

/// <summary>
/// Represents configuration options.
/// </summary>
public record ScalingOptions(
    string Organization,
    string PersonalAccessToken,
    string PoolName,
    int MinAgents,
    int MaxAgents,
    int Interval,
    string DockerImageName
);
