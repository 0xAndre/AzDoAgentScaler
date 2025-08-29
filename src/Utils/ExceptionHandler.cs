namespace AzDoAgentScaler.Utils;

/// <summary>
/// Handles unhandled exceptions in a consistent way across the application.
/// </summary>
public static class ExceptionHandler
{
    /// <summary>
    /// Registers global exception handlers for the application.
    /// </summary>
    public static void RegisterGlobal()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                ConsoleHelper.Error($"Unhandled exception: {ex.Message}");
                ConsoleHelper.Debug(ex.ToString());
            }
            else
            {
                ConsoleHelper.Error("Unhandled exception: unknown error.");
            }

            Environment.Exit(1);
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            ConsoleHelper.Error($"Unobserved task exception: {e.Exception.Message}");
            ConsoleHelper.Debug(e.Exception.ToString());
            e.SetObserved();
        };
    }

    /// <summary>
    /// Executes an action with exception handling.
    /// </summary>
    public static void SafeRun(Action action, string context = "operation")
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Error during {context}: {ex.Message}");
            ConsoleHelper.Debug(ex.ToString());
        }
    }

    /// <summary>
    /// Executes a task with exception handling.
    /// </summary>
    public static async Task SafeRunAsync(Func<Task> action, string context = "operation")
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error($"Error during {context}: {ex.Message}");
            ConsoleHelper.Debug(ex.ToString());
        }
    }
}
