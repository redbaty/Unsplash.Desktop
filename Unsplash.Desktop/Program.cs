using System;

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
            //TODO: better handling of command args, (handle help (--help /?) etc.)
            string mode = args.Length > 0 ? args[0] : ""; //default to gui

            if (mode == "-h")
            {
                Core.Unsplash.Main(args);
            }
            else
            {
                ConsoleManager.Show();
                Core.Unsplash.Main(args);
                Console.ReadKey();
            }
        }
    }
}