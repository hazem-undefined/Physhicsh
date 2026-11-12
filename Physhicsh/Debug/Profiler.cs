using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Physhicsh.Debug;

public class Profiler
{
    private readonly Stopwatch _sw;
    private readonly Dictionary<string, double> _samples = [];
    public IReadOnlyDictionary<string, double> Samples => _samples;
    /// <summary>
    /// Returns 0 if not measured.
    /// </summary>
    public double this[string name] => _samples.TryGetValue(name, out double ms) ? ms : 0;
    public bool Has(string name) => _samples.ContainsKey(name);
    public void Measure(string name, Action action)
    {
        long start = Stopwatch.GetTimestamp();
        try { action(); }
        finally { _samples[name] = Stopwatch.GetElapsedTime(start).TotalMilliseconds; }
    }
    public T Measure<T>(string name, Func<T> func)
    {
        long start = Stopwatch.GetTimestamp();
        try { return func(); }
        finally { _samples[name] = Stopwatch.GetElapsedTime(start).TotalMilliseconds; }
    }
    public void Remove(string name) => _samples.Remove(name);
    public void Reset() => _samples.Clear();
}
