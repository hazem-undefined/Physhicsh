using System;
using System.IO;
using System.Threading;

namespace Physhicsh.Debug;

public sealed class Log
{
    public enum Level { Info, Warning, Error }
    private static readonly Lock _writeLock = new();
    private static readonly string _filePath;

    static Log()
    {
        DateTime now = DateTime.Now;
        string fileName = $"{now:yyyy-MM-dd}-{now:HH-mm}.log";
        string logDir = Path.Combine(AppContext.BaseDirectory, "log");

        Directory.CreateDirectory(logDir);
        _filePath = Path.Combine(logDir, fileName);

        try { File.AppendAllText(_filePath, string.Empty); }
        catch { /* Nothing ever happens */ }
    }

    public static void Write(Level level, string msg)
    {
        string line = $"[{level.ToString().ToUpperInvariant()}]: {msg}";
        try
        {
            lock (_writeLock)
                File.AppendAllText(_filePath, line + Environment.NewLine);
        }
        catch { /* Nothing ever happens */ }
    }

    public static void Info(string msg) => Write(Level.Info, msg);
    public static void Warning(string msg) => Write(Level.Warning, msg);
    public static void Error(string msg) => Write(Level.Error, msg);
}