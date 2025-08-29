namespace AzDoAgentScaler.Services.Interfaces;

/// <summary>
/// Defines operations for managing Azure DevOps agent containers via Docker.
/// </summary>
public interface IDockerService
{
    /// <summary>
    /// Creates a new Docker container running an Azure DevOps agent.
    /// </summary>
    /// <param name="organization">The Azure DevOps organization name.</param>
    /// <param name="personalAccessToken">The Personal Access Token (PAT) for authentication.</param>
    /// <param name="poolName">The name of the agent pool.</param>
    /// <param name="agentName">The name to assign to the agent container.</param>
    /// <param name="dockerImage">The Docker image to use for the agent.</param>
    Task CreateAgentAsync(string organization, string personalAccessToken, string poolName, string agentName, string dockerImage);

    /// <summary>
    /// Removes a Docker container by its agent name.
    /// </summary>
    /// <param name="agentName">The name of the agent (and container) to remove.</param>
    Task RemoveAgentAsync(string agentName);
}
