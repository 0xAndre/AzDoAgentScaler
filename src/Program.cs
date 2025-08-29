using System.CommandLine;
using System.Reflection;
using AzDoAgentScaler.Services;
using AzDoAgentScaler.Services.Interfaces;
using AzDoAgentScaler.Utils;


var assembly = Assembly.GetExecutingAssembly();

var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
              ?? assembly.GetName().Version?.ToString();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine(@"
    _         ____          _                    _   ____            _           
   / \    ___|  _ \  ___   / \   __ _  ___ _ __ | |_/ ___|  ___ __ _| | ___ _ __ 
  / _ \  |_  / | | |/ _ \ / _ \ / _` |/ _ \ '_ \| __\___ \ / __/ _` | |/ _ \ '__|
 / ___ \  / /| |_| | (_) / ___ \ (_| |  __/ | | | |_ ___) | (_| (_| | |  __/ |   
/_/   \_\/___|____/ \___/_/   \_\__, |\___|_| |_|\__|____/ \___\__,_|_|\___|_|   
                                |___/
");

Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine("Auto-scaling Azure DevOps agents");
Console.WriteLine($"Version: {version}\n");
Console.ResetColor();

// Register global exception handlers
ExceptionHandler.RegisterGlobal();

// Define CLI options
var orgOption = new Option<string>("--org", "Azure DevOps organization name") { IsRequired = true };
var patOption = new Option<string>("--pat", "Personal Access Token") { IsRequired = true };
var poolNameOption = new Option<string>("--poolName", "Agent Pool Name") { IsRequired = true };
var minAgentsOption = new Option<int>("--minAgents", () => 1, "Minimum number of agents");
var maxAgentsOption = new Option<int>("--maxAgents", () => 5, "Maximum number of agents");
var intervalOption = new Option<int>("--interval", () => 30, "Polling interval in seconds");
var dockerImageOption = new Option<string>("--dockerImage", () => "mcr.microsoft.com/azure-pipelines/vsts-agent", "Docker image name (Docker Hub)");

// Validators
minAgentsOption.AddValidator(result =>
{
    if (result.GetValueOrDefault<int>() <= 0)
        result.ErrorMessage = "The minimum number of agents must be greater than zero.";
});
maxAgentsOption.AddValidator(result =>
{
    if (result.GetValueOrDefault<int>() <= 0)
        result.ErrorMessage = "The maximum number of agents must be greater than zero.";
});
intervalOption.AddValidator(result =>
{
    if (result.GetValueOrDefault<int>() <= 0)
        result.ErrorMessage = "The polling interval must be greater than zero.";
});

// Create RootCommand
var rootCommand = new RootCommand("CLI for auto-scaling Azure DevOps agents")
{
    orgOption,
    patOption,
    poolNameOption,
    minAgentsOption,
    maxAgentsOption,
    intervalOption,
    dockerImageOption
};

// Set handler using explicit options
rootCommand.SetHandler(
    async (
        string org,
        string pat,
        string poolName,
        int minAgents,
        int maxAgents,
        int interval,
        string dockerImage
    ) =>
    {
        var scalingOptions = new ScalingOptions(org, pat, poolName, minAgents, maxAgents, interval, dockerImage);

        using var httpClient = new HttpClient();

        // Create services
        IAzureDevOpsService azdoService = new AzureDevOpsService(httpClient, org, pat);
        IDockerService dockerService = new DockerService();

        // Initialize the AgentScaler with services and options
        var scaler = new AgentScaler(
            azdoService, 
            dockerService, 
            scalingOptions
           );

        // Enable cancellation for Ctrl + C
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
            ConsoleHelper.Warn("Cancellation requested... exiting gracefully.");
        };

        await scaler.RunAsync(cts.Token);
    },
    orgOption, patOption, poolNameOption, minAgentsOption, maxAgentsOption, intervalOption, dockerImageOption
);

// Invoke CLI
return await rootCommand.InvokeAsync(args);
