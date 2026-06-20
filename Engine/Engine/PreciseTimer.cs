using System.Runtime.InteropServices;
using System.Security;

namespace Engine;

public class PreciseTimer
{
    private long _ticksPerSecond;

    private long _previousElapsedTime;

    [DllImport("kernel32")]
    [SuppressUnmanagedCodeSecurity]
    private static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

    [DllImport("kernel32")]
    [SuppressUnmanagedCodeSecurity]
    private static extern bool QueryPerformanceCounter(ref long PerformanceCount);

    public PreciseTimer()
    {
        QueryPerformanceFrequency(ref _ticksPerSecond);
        GetElapsedTime();
    }

    public double GetElapsedTime()
    {
        long PerformanceCount = 0L;
        QueryPerformanceCounter(ref PerformanceCount);
        double result = (double)(PerformanceCount - _previousElapsedTime) / (double)_ticksPerSecond;
        _previousElapsedTime = PerformanceCount;
        return result;
    }
}
