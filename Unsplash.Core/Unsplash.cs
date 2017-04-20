using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
            Console.WriteAscii("Unsplash", Color.MediumSlateBlue);
            Console.WriteLine("Version 1.1", Color.YellowGreen);
            Console.WriteLine("Help us @ https://github.com/redbaty/Unsplash.Desktop\n", Color.Gray);

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
                Console.WriteLine($"> Downloading wallpaper image", Color.Gray);
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
                    // ignored
                }

                var td = ts.NewTask();
                td.Principal.RunLevel = TaskRunLevel.Highest;
                td.RegistrationInfo.Description =
                    "Changes wallpaper using unsplash. Help us @ https://github.com/redbaty/Unsplash.Desktop";
                td.Triggers.Add(new DailyTrigger
                {
                    StartBoundary = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                    Repetition = new RepetitionPattern(new TimeSpan(0, 0, 10, 0), TimeSpan.Zero)
                });
                td.Actions.Add(new ExecAction(Assembly.GetEntryAssembly().Location, "-h"));
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

        static UnsplashSource GenerateUnsplashSource()
        {
            var menu = ShowEnumMenu<UnsplashSourceType>();
            Console.WriteLine(menu);
            switch (menu)
            {
                case UnsplashSourceType.Random:
                    return new RandomUnsplashSource();
                case UnsplashSourceType.Category:
                    return new CategoryUnsplashSource();
                case UnsplashSourceType.Collection:
                    return new CollectionUnsplashSource();
                case UnsplashSourceType.SearchTerm:
                    return new SearchtermUnsplashSource(true);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}