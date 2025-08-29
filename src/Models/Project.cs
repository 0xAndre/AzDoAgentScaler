
using System.Text.Json.Serialization;

namespace AzDoAgentScaler.Models;

/// <summary>
/// Represents an Azure DevOps project.
/// A project is a container for repositories, pipelines, boards, and other resources in Azure DevOps.
/// </summary>
public class Project
{
    /// <summary>
    /// Unique identifier of the project (GUID).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable name of the project.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the project.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// URL of the project in Azure DevOps.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Current state of the project (e.g., "wellFormed", "deleting").
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Revision number of the project.
    /// </summary>
    [JsonPropertyName("revision")]
    public int Revision { get; set; }

    /// <summary>
    /// Visibility of the project (e.g., "private", "public").
    /// </summary>
    [JsonPropertyName("visibility")]
    public string Visibility { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of the last update made to the project.
    /// </summary>
    [JsonPropertyName("lastUpdateTime")]
    public DateTime LastUpdateTime { get; set; }
}
