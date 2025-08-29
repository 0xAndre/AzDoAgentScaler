using System.Text.Json.Serialization;

namespace AzDoAgentScaler.Models;

/// <summary>
/// Represents the queue used by a build in Azure DevOps.
/// </summary>
public class Queue
{
    /// <summary>
    /// Unique identifier of the queue.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Human-readable name of the queue.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

