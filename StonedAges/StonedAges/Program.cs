using System;
using System.Windows.Forms;

namespace StonedAges;

/// <summary>Application entry point.</summary>
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(defaultValue: false);
        Application.Run(new GameWindow());
    }
}
