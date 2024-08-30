using System.Runtime.CompilerServices;

namespace ProxyProtocolSocket.Utils
{
    public enum LogLevel
    {
        None,
        Error,
        Warning,
        Info,
        Debug,
    }

    public static class Logger
    {
        private static readonly Dictionary<LogLevel, ConsoleColor> _level2Color = new()
        {
            { LogLevel.None,    ConsoleColor.White },
            { LogLevel.Error,   ConsoleColor.Red },
            { LogLevel.Warning, ConsoleColor.Yellow },
            { LogLevel.Info,    ConsoleColor.White },
            { LogLevel.Debug,   ConsoleColor.Green }
        };

        private static readonly Dictionary<LogLevel, bool> _level2ShowCaller = new()
        {
            { LogLevel.None,    false },
            { LogLevel.Error,   true },
            { LogLevel.Warning, true },
            { LogLevel.Info,    false },
            { LogLevel.Debug,   true }
        };

        private static readonly object _consoleWriteLock = new();

        public static void Log(string content, LogLevel level = LogLevel.Debug, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNum = 0)
        {
            string fileName = Path.GetFileName(filePath);

            if (level > ProxyProtocolSocketPlugin.Config.Settings.LogLevel)
                return;
            lock (_consoleWriteLock)
            {
                Console.ForegroundColor = _level2Color[level];
                Console.WriteLine($"[ProxyProtocolSocket] [{level:G}]{(_level2ShowCaller[level] ? $" [{fileName}:{lineNum}]" : "")} {content}");
                Console.ResetColor();
            }
        }
    }
}
