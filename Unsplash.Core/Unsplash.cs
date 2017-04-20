using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using Unsplash.Core.Enums;
using Unsplash.Core.Extensions;
using Unsplash.Core.Sources;
using Console = Colorful.Console;

namespace Unsplash.Core
{
    public class Unsplash
    {
        public static Settings Settings { get; set; }

        public static JsonSerializerSettings JsonSettings { get; } =
            new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};

        public static void Main(string[] args)
        {
            Console.WriteAscii("Unsplash", Color.MediumSlateBlue);
            Console.WriteLine("Version 1.2.5", Color.YellowGreen);
            Console.WriteLine("Help us @ https://github.com/redbaty/Unsplash.Desktop\n", Color.Gray);

            if (args.Length == 0 || args[0] != "-g")
                try
                {
                    Settings =
                        JsonConvert.DeserializeObject<Settings>(
                            File.ReadAllText(Environment.ExpandEnvironmentVariables("%USERPROFILE%\\Unsplash.desktop")),
                            JsonSettings
                        );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("> Failed to load settings.", Color.Crimson);
                    Console.WriteLine(ex.Message, Color.Crimson);
                    Console.WriteLine("> Run the program with -g to generate another one.", Color.CadetBlue);
                    return;
                }
            else if (args.Length != 0 && args[0] == "-g")
            {
                GenerateSettings();
            }

            Console.WriteLine("> Downloading wallpaper image...", Color.Gray);
            Wallpaper.Set(
                new Uri(Settings.Source.BuildUrlString(Settings)),
                Settings.WallpaperDisplayStyle);
            Console.WriteLine("> New wallpaper set!", Color.LimeGreen);
            Console.WriteLine("> Press any key to continue", Color.Gray);
        }

        public static T ShowEnumMenu<T>()
        {
            var enumValues = Enum.GetValues(typeof(T)).OfType<Enum>().ToList();
            Console.WriteLine($"Select a {typeof(T)} member");

            for (var i = 0; i < enumValues.Count; i++)
            {
                var member = enumValues[i];
                var description = member.GetEnumDescription() == member.ToString()
                    ? ""
                    : " // " + member.GetEnumDescription();
                Console.WriteLine($"[{i}] {member}{description}");
            }
            var id = Questions.AskIntMessage("Pick a enum member (eg: 0): ", Color.Gray);
            return (T) Enum.Parse(typeof(T), id.ToString());
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
                    Repetition = new RepetitionPattern(Settings.Interval, TimeSpan.Zero)
                });
                td.Actions.Add(new ExecAction(Assembly.GetEntryAssembly().Location, "-h"));
                ts.RootFolder.RegisterTaskDefinition("Update wallpaper", td);
            }
        }

        static void GenerateSettings()
        {
            var settings = new Settings
            {
                ImageWidth = Questions.AskIntMessage("> Enter the desired image width: ", Color.Gray),
                ImageHeight = Questions.AskIntMessage("> Enter the desired image height: ", Color.Gray),
                WallpaperDisplayStyle = ShowEnumMenu<WallpaperDisplayStyle>(),
                Source = GenerateUnsplashSource(),
                Interval = new TimeSpan(0, 0,
                    Questions.AskIntMessage("How often the wallpaper should be updated (In minutes)? "), 0, 0)
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
                    return new CategoryUnsplashSource(true);
                case UnsplashSourceType.Collection:
                    return new CollectionUnsplashSource(true);
                case UnsplashSourceType.SearchTerm:
                    return new SearchtermUnsplashSource(true);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}