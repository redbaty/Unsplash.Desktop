using System;
using System.Drawing;
using System.IO;
using System.Net;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using Console = Colorful.Console;

namespace Unsplash.Core
{
    public class Unsplash
    {
        public static Settings Settings { get; set; }

        public static void Main(string[] args)
        {
            //AddToPath();
            Console.WriteLine($"Unsplash Desktop v1.0\n", Color.MediumSlateBlue);

            if (args.Length == 0 || args[0] != "-g")
                try
                {
                    Settings =
                        JsonConvert.DeserializeObject<Settings>(
                            File.ReadAllText(Environment.ExpandEnvironmentVariables("%USERPROFILE%\\Unsplash.desktop"))
                        );
                }
                catch
                {
                    Console.WriteLine("> Failed to load settings.", Color.Crimson);
                    Console.WriteLine("> Run the program with -g to generate another one.", Color.CadetBlue);
                    return;
                }
            else if (args.Length != 0 && args[0] == "-g")
            {
                GenerateSettings();
            }

            using (var webclient = new WebClient())
            {
                Console.WriteLine($"> Downloading new desktop", Color.Gray);
                webclient.DownloadFile(
                    $"https://source.unsplash.com/random/{Settings.ImageWidth}x{Settings.ImageHeight}",
                    "wallpaper.jpg");
                var envr = $"{Environment.CurrentDirectory}\\wallpaper.jpg";
                Wallpaper.Set(new Uri(envr), Wallpaper.Style.Centered);
                Console.WriteLine($"> New wallpaper set", Color.YellowGreen);
            }
        }

        private static void AddToPath()
        {
            var name = "PATH";
            var value = System.Reflection.Assembly.GetEntryAssembly().Location;
            var target = EnvironmentVariableTarget.Machine;
            Environment.SetEnvironmentVariable(name, value, target);
        }

        private static void CreateLoopTask()
        {
            using (var ts = new TaskService())
            {
                try
                {
                    ts.RootFolder.DeleteTask("Update wallpaper");
                }
                catch
                {
                }

                var td = ts.NewTask();

                td.Principal.RunLevel = TaskRunLevel.Highest;
                td.RegistrationInfo.Description = "Changes wallpaper using unsplash. (GITHUB_PAGE)";
                td.Triggers.Add(new DailyTrigger
                {
                    StartBoundary = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    Repetition = new RepetitionPattern(new TimeSpan(0, 0, 10, 0), TimeSpan.Zero)
                });
                td.Actions.Add(new ExecAction(System.Reflection.Assembly.GetEntryAssembly().Location, "-h"));
                ts.RootFolder.RegisterTaskDefinition("Update wallpaper", td);
            }
        }

        static void GenerateSettings()
        {
            var settings = new Settings
            {
                ImageWidth = IntMessage("> Enter the image width: ", Color.Gray),
                ImageHeight = IntMessage("> Enter the image height: ", Color.Gray)
            };
            Settings = settings;
            settings.Save();
            CreateLoopTask();
        }

        static int IntMessage(string message, Color color)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine(message, color);
                    var stringinput = Console.ReadLine();
                    return Convert.ToInt32(stringinput);
                }
                catch
                {
                    Console.WriteLine("> Couldn't convert the input to int (Only Numbers)", Color.Crimson);
                }
            }
        }
    }
}