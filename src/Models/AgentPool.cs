namespace AzDoAgentScaler.Models;

/// <summary>
/// Represents an Azure DevOps agent pool.
/// An agent pool is a collection of self-hosted agents used to run pipeline jobs.
/// </summary>
public class AgentPool
{
    /// <summary>
    /// Unique identifier of the agent pool.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Human-readable name of the agent pool.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}



