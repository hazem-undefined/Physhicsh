using System;
using System.IO;

namespace Physhicsh.Debug;

public sealed class Log
{
    public static bool UseConsole = false;
    public enum Level { Info, Warning, Error }

    private static readonly Lazy<Log> _instance = new(() => new Log());
    public static Log Instance => _instance.Value;

    private readonly object _writeLock = new();
    private readonly string _filePath;

    private Log()
    {
        if (UseConsole) return;

        DateTime now = DateTime.Now;
        //                  date-hour:minute.log
        string fileName = $"{now:yyyy-MM-dd}-{now:HH-mm}.log";
        _filePath = Path.Combine([AppContext.BaseDirectory, "log", fileName]);

        File.AppendAllText(_filePath, string.Empty);
    }

    public static void Write(Level level, string msg)
    {
        string line = $"[{level.ToString().ToUpperInvariant()}]: {msg}";

        if (UseConsole)
        {
            Console.WriteLine(line);
            return;
        }

        Log log = _instance.Value;

        lock (log._writeLock)
        {
            File.AppendAllText(log._filePath, line + Environment.NewLine);
        }
    }

    public static void Info(string msg) => Write(Level.Info, msg);
    public static void Warning(string msg) => Write(Level.Warning, msg);
    public static void Error(string msg) => Write(Level.Error, msg);
}