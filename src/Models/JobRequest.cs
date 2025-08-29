namespace AzDoAgentScaler.Models;

/// <summary>
/// Represents a job request in an Azure DevOps agent pool.
/// </summary>
public class JobRequest
{
    /// <summary>
    /// The result of the job request (null if not completed).
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// The time the job was assigned to an agent (null if not assigned yet).
    /// </summary>
    public DateTime? AssignTime { get; set; }
}