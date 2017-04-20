using System;
using System.Windows.Forms;

namespace Unsplash.Desktop
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var mode = args.Length > 0 ? args[0] : "";
            if (mode == "-h")
            {
                Core.Unsplash.Main(args);
            }
            else
            {
                ConsoleManager.Show();
                Core.Unsplash.Main(args);
                ConsoleManager.Free();
                Application.Exit();
            }
        }
    }
}