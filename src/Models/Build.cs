using System.Text.Json.Serialization;

namespace AzDoAgentScaler.Models;

/// <summary>
/// Represents an Azure DevOps build.
/// A build is the result of a pipeline execution in Azure DevOps.
/// </summary>
public class Build
{
    /// <summary>
    /// Unique identifier of the build.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// The queue information associated with this build.
    /// This indicates which agent pool or queue the build is using.
    /// </summary>
    [JsonPropertyName("queue")]
    public Queue? Queue { get; set; }
}

