﻿using System;
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
                Wallpaper.Set(new Uri(envr), Settings.WallpaperStyle);
                File.Delete(envr);

                Console.WriteLine($"> New wallpaper set!", Color.YellowGreen);
            }
        }

        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes =
                (DescriptionAttribute[]) fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            return value.ToString();
        }

        private static Wallpaper.Style StyleMenu(Type enume)
        {
            var x = Enum.GetValues(enume).OfType<Enum>().ToList();
            Console.WriteLine("");
            for (var i = 0; i < x.Count; i++)
            {
                var member = x[i];
                var description = GetEnumDescription(member) == member.ToString()
                    ? ""
                    : " // " + GetEnumDescription(member);
                Console.WriteLine($"[{i}] {member}{description}");
            }
            var id = IntMessage("Pick a wallpaper display mode (eg: 0): ", Color.Gray);
            return (Wallpaper.Style) id;
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
                ImageWidth = IntMessage("> Enter the desired image width: ", Color.Gray),
                ImageHeight = IntMessage("> Enter the desired image height: ", Color.Gray),
                WallpaperStyle = StyleMenu(typeof(Wallpaper.Style))
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