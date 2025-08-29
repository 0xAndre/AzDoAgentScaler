using System.Diagnostics;
using AzDoAgentScaler.Services.Interfaces;
using AzDoAgentScaler.Utils;

namespace AzDoAgentScaler.Services;

public class DockerService : IDockerService
{
    public async Task CreateAgentAsync(string organization, string personalAccessToken, string poolName, string agentName, string dockerImage)
    {
        try
        {
            var arguments =
                $"run -d --name {agentName} " +
                $"-e VSTS_ACCOUNT={organization} " +
                $"-e VSTS_TOKEN={personalAccessToken} " +
                $"-e VSTS_POOL={poolName} " +
                $"-e VSTS_AGENT={agentName} " +
                $"-e VSTS_AGENT_OS=linux " +
                $"{dockerImage}";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new Exception($"Docker run failed: {error}");
            }

            ConsoleHelper.Info($"Docker container '{agentName}' created successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Failed to create Docker agent '{agentName}': {ex.Message}");
        }
    }

    public async Task RemoveAgentAsync(string agentName)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"rm -f {agentName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new Exception($"Docker remove failed: {error}");
            }

            ConsoleHelper.Info($"Docker container '{agentName}' removed successfully.");
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Failed to remove Docker agent '{agentName}': {ex.Message}");
        }
    }
}
