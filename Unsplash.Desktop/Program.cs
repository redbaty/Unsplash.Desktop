using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Console = Colorful.Console;

namespace Unsplash.Desktop
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static async Task Main(string[] args)
        {
            var mode = args.Length > 0 ? args[0] : "";
            var service = new Core.Unsplash(new ConsoleLogger());

            if (mode == "-h")
            {
                await service.RefreshWallpaper(SettingsHelper.LoadSettings());
            }
            else
            {
                ConsoleManager.Show();

                Console.WriteAscii("Unsplash", Color.MediumSlateBlue);
                Console.WriteLine("Version 1.2.5", Color.YellowGreen);
                Console.WriteLine("Help us @ https://github.com/redbaty/Unsplash.Desktop\n", Color.Gray);

                if (mode == "-g")
                {
                    var settings = SettingsHelper.GenerateSettings();
                    SettingsHelper.SaveSettings(settings);
                    service.CreateLoopTask(settings);
                }
                else
                {
                    await service.RefreshWallpaper(SettingsHelper.LoadSettings());
                    ConsoleManager.Free();
                    Application.Exit();
                }
            }
        }
    }
}