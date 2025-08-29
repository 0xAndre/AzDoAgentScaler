using AzDoAgentScaler.Models;

namespace AzDoAgentScaler.Services.Interfaces;

/// <summary>
/// Defines operations for interacting with the Azure DevOps API related to agent pools.
/// </summary>
public interface IAzureDevOpsService
{
    /// <summary>
    /// Gets the ID of an agent pool by its name.
    /// </summary>
    /// <param name="poolName">The name of the agent pool.</param>
    /// <returns>The pool ID if found; otherwise, null.</returns>
    Task<int?> GetPoolIdByNameAsync(string poolName);

    /// <summary>
    /// Gets the number of waiting jobs (not assigned and not completed) in a pool.
    /// </summary>
    /// <param name="poolId">The ID of the agent pool.</param>
    /// <returns>The number of waiting jobs.</returns>
    Task<int> GetWaitingJobsAsync(int poolId);

    /// <summary>
    /// Gets the number of online and enabled agents in a pool.
    /// </summary>
    /// <param name="poolId">The ID of the agent pool.</param>
    /// <returns>The number of online agents.</returns>
    Task<int> GetOnlineAgentsAsync(int poolId);

    /// <summary>
    /// Finds an idle agent (enabled, online, and not currently assigned to a job) in a pool.
    /// </summary>
    /// <param name="poolId">The ID of the agent pool.</param>
    /// <returns>A tuple with the agent ID and name, or null if none found.</returns>
    Task<(int Id, string Name)?> GetIdleAgentAsync(int poolId);

    /// <summary>
    /// Removes an agent from the specified pool using the Azure DevOps API.
    /// </summary>
    /// <param name="poolId">The ID of the agent pool.</param>
    /// <param name="agentId">The ID of the agent to remove.</param>
    /// <returns><c>true</c> if the removal succeeded; otherwise, <c>false</c>.</returns>
    Task<bool> RemoveAgentFromPoolAsync(int poolId, int agentId);
}
