using System.Text.Json.Serialization;

namespace AzDoAgentScaler.Models;

/// <summary>
/// Represents an Azure DevOps release.
/// A release is a deployment of an application or infrastructure to one or more environments.
/// </summary>
public class Release
{
    /// <summary>
    /// Unique identifier of the release.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Human-readable name of the release.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The queue associated with the release.
    /// Indicates which agent pool or pipeline queue is used for this release.
    /// </summary>
    [JsonPropertyName("queue")]
    public Queue? Queue { get; set; }
}


