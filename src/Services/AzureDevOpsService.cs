using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AzDoAgentScaler.Models;
using AzDoAgentScaler.Services.Interfaces;
using AzDoAgentScaler.Utils;

namespace AzDoAgentScaler.Services;

public class AzureDevOpsService : IAzureDevOpsService
{
    private readonly HttpClient _httpClient;
    private readonly string _organization;

    public AzureDevOpsService(HttpClient httpClient, string organization, string personalAccessToken)
    {
        _httpClient = httpClient;
        _organization = organization;

        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
    }

    public async Task<int?> GetPoolIdByNameAsync(string poolName)
    {
        try
        {
            var url = $"https://dev.azure.com/{_organization}/_apis/distributedtask/pools?api-version=7.1";
            var response = await _httpClient.GetStringAsync(url);

            var pools = JsonSerializer.Deserialize<ApiResponse<AgentPool>>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            var pool = pools.Value.FirstOrDefault(p => p.Name.Equals(poolName, StringComparison.OrdinalIgnoreCase));
            return pool?.Id;
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Failed to get pool ID: {ex.Message}");
            return null;
        }
    }

    public async Task<int> GetWaitingJobsAsync(int poolId)
    {
        try
        {
            var url = $"https://dev.azure.com/{_organization}/_apis/distributedtask/pools/{poolId}/jobrequests?api-version=7.1";
            var response = await _httpClient.GetStringAsync(url);

            var jobs = JsonSerializer.Deserialize<ApiResponse<JobRequest>>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            return jobs.Value.Count(j => j.Result == null && j.AssignTime == null);
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Failed to get waiting jobs: {ex.Message}");
            return 0;
        }
    }

    public async Task<int> GetOnlineAgentsAsync(int poolId)
    {
        try
        {
            var url = $"https://dev.azure.com/{_organization}/_apis/distributedtask/pools/{poolId}/agents?api-version=7.1";
            var response = await _httpClient.GetStringAsync(url);

            var agents = JsonSerializer.Deserialize<ApiResponse<Agent>>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            return agents.Value.Count(a => a.Enabled && a.Status == "online");
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Failed to get online agents: {ex.Message}");
            return 0;
        }
    }

    public async Task<(int Id, string Name)?> GetIdleAgentAsync(int poolId)
    {
        try
        {
            var url = $"https://dev.azure.com/{_organization}/_apis/distributedtask/pools/{poolId}/agents?includeAssignedRequest=true&api-version=7.1";
            var response = await _httpClient.GetStringAsync(url);

            var agents = JsonSerializer.Deserialize<ApiResponse<Agent>>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            var idle = agents.Value.FirstOrDefault(a => a.Enabled && a.Status == "online" && a.AssignedRequest == null);

            if (idle != null)
            {
                return (idle.Id, idle.Name);
            }
                
            return null;
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Failed to get idle agent: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> RemoveAgentFromPoolAsync(int poolId, int agentId)
    {
        try
        {
            var url = $"https://dev.azure.com/{_organization}/_apis/distributedtask/pools/{poolId}/agents/{agentId}?api-version=7.1";
            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                ConsoleHelper.Info($"Agent {agentId} removed from pool successfully.");
                return true;
            }
            else
            {
                ConsoleHelper.Error($"Failed to remove agent {agentId}. Status: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Failed to remove agent {agentId}: {ex.Message}");
            return false;
        }
    }
}
