namespace AzDoAgentScaler.Utils;

/// <summary>
/// Provides helper methods for color-coded console output.
/// </summary>
public static class ConsoleHelper
{
    private static readonly object _lock = new();

    public static void Info(string message) =>
        WriteColored(message, ConsoleColor.Cyan);

    public static void Success(string message) =>
        WriteColored(message, ConsoleColor.Green);

    public static void Warn(string message) =>
        WriteColored(message, ConsoleColor.Yellow);

    public static void Error(string message) =>
        WriteColored(message, ConsoleColor.Red);

    public static void Debug(string message)
    {
#if DEBUG
        WriteColored(message, ConsoleColor.DarkGray);
#endif
    }

    private static void WriteColored(string message, ConsoleColor color)
    {
        lock (_lock)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = original;
        }
    }
}
