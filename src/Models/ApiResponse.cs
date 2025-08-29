namespace AzDoAgentScaler.Models;

/// <summary>
/// Represents a response from Azure DevOps REST API that contains a list of items.
/// </summary>
/// <typeparam name="T">Type of the items in the response.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// List of items returned by the API.
    /// </summary>
    public List<T> Value { get; set; } = new List<T>();

    /// <summary>
    /// Total count of items returned by the API.
    /// </summary>
    public int Count { get; set; }
}
