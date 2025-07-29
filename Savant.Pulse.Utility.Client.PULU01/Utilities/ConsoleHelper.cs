using System.Runtime.InteropServices;

namespace Savant.Pulse.Utility.Client.PULU01.Utilities;

public static class ConsoleHelper
{
    private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    private static readonly bool UseAsciiMode = IsWindows && !IsWindowsTerminal();

    public static ConsoleIcons Icons => UseAsciiMode ? ConsoleIcons.Ascii : ConsoleIcons.Unicode;

    private static bool IsWindowsTerminal()
    {
        // Check if running in Windows Terminal (which has better Unicode support)
        var wtSession = Environment.GetEnvironmentVariable("WT_SESSION");
        return !string.IsNullOrEmpty(wtSession);
    }
}

public class ConsoleIcons
{
    public string Success { get; init; } = string.Empty;
    public string Failed { get; init; } = string.Empty;
    public string Skipped { get; init; } = string.Empty;
    public string Speed { get; init; } = string.Empty;
    public string Complete { get; init; } = string.Empty;
    public string Stopped { get; init; } = string.Empty;
    public string Stats { get; init; } = string.Empty;
    public string Duration { get; init; } = string.Empty;
    public string ErrorLog { get; init; } = string.Empty;
    public string SuccessLog { get; init; } = string.Empty;
    public char ProgressFilled { get; init; }
    public char ProgressEmpty { get; init; }

    public static readonly ConsoleIcons Unicode = new()
    {
        Success = "✅",
        Failed = "❌", 
        Skipped = "⏭️",
        Speed = "🚀",
        Complete = "🎉",
        Stopped = "⏹️",
        Stats = "📊",
        Duration = "⏱️",
        ErrorLog = "📋",
        SuccessLog = "📋",
        ProgressFilled = '█',
        ProgressEmpty = '░'
    };

    public static readonly ConsoleIcons Ascii = new()
    {
        Success = "OK",
        Failed = "ERR",
        Skipped = "SKIP",
        Speed = "RATE",
        Complete = "DONE",
        Stopped = "STOP",
        Stats = "STATS",
        Duration = "TIME",
        ErrorLog = "ERRORS",
        SuccessLog = "SUCCESS",
        ProgressFilled = '=',
        ProgressEmpty = '-'
    };
}