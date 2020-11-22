using System;
using JCIW.App;
using JCIW.Data;
using Client_Networking.Desktop;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Client_Desktop
{
    /// <summary>
    /// Main class for client desktop.
    /// </summary>
    static class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
                ShowWindow(h, 0);
            }
            catch
            {
                // Only work on Windows...
            }

            MainApp app = new MainApp(new ClientNetwork(), new DesktopFunctions(), Platform.Desktop);
            using (Game game = (Game)app.Window())
            {
                game.Run();
            }
        }
    }
}
