using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Unsplash.Desktop
{
    [SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32", SetLastError = true)]
        static extern bool AttachConsole(int dwProcessId);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            var ptr = GetForegroundWindow();
            GetWindowThreadProcessId(ptr, out int u);
            var process = Process.GetProcessById(u);

            if (process.ProcessName == "cmd") //Is the uppermost window a cmd process?
            {
                AttachConsole(process.Id);
                Console.WriteLine("");
            }
            else
            {
                AllocConsole();
            }
        }

        public static void Free()
        {
            FreeConsole();
        }
    }
}