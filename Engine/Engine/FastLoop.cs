using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace Engine;

public class FastLoop
{
    public struct Message
    {
        public IntPtr hWnd;

        public int msg;

        public IntPtr wParam;

        public IntPtr lParam;

        public uint time;

        public System.Drawing.Point p;
    }

    public delegate void LoopCallback(double elapsedTime);

    private PreciseTimer _timer = new PreciseTimer();

    private LoopCallback _callback;

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    [SuppressUnmanagedCodeSecurity]
    public static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

    public FastLoop(LoopCallback callback)
    {
        _callback = callback;
        Application.Idle += OnApplicationEnterIdle;
    }

    private void OnApplicationEnterIdle(object sender, EventArgs e)
    {
        while (IsAppStillIdle())
        {
            _callback(_timer.GetElapsedTime());
        }
    }

    private bool IsAppStillIdle()
    {
        Message msg;
        return !PeekMessage(out msg, IntPtr.Zero, 0u, 0u, 0u);
    }
}
