using System.Diagnostics;
using System;

/// <summary>
/// Use this class similar to how you would use Stopwatch. The only addition is the ability
/// to get the Elapsed time in a readable format easily.
/// </summary>
public class PerformanceStopwatch : Stopwatch
{
    /// <summary>
    /// Gets the Elapsed time of the stopwatch in a readable format
    /// "{0:00}:{1:00}:{2:00}.{3:00}"
    /// </summary>
    /// <returns></returns>
    public string GetElapsedTime()
    {
        TimeSpan ts = this.Elapsed;
        return string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        ts.Hours, ts.Minutes, ts.Seconds,
        ts.Milliseconds / 10);
    }
}