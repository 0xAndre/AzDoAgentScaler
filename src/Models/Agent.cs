namespace AzDoAgentScaler.Models;

/// <summary>
/// Represents an Azure DevOps self-hosted agent.
/// A self-hosted agent is a machine that runs pipeline jobs in Azure DevOps.
/// </summary>
public class Agent
{
    /// <summary>
    /// Unique identifier of the agent.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Human-readable name of the agent.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the agent is enabled and available to run jobs.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Current status of the agent (e.g., "online", "offline", "busy").
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// The job request currently assigned to this agent, if any.
    /// Null if the agent is idle.
    /// </summary>
    public AssignedRequest? AssignedRequest { get; set; }
}

/// <summary>
/// Represents a job request assigned to an agent.
/// </summary>
public class AssignedRequest
{
    /// <summary>
    /// Unique identifier of the job request.
    /// </summary>
    public int RequestId { get; set; }

    /// <summary>
    /// The time the job was queued or assigned to the agent.
    /// </summary>
    public DateTime QueueTime { get; set; }

    /// <summary>
    /// The service or user that owns the job request.
    /// </summary>
    public string ServiceOwner { get; set; } = string.Empty;
}