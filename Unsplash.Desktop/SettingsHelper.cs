using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unsplash.Core;
using Unsplash.Core.Enums;
using Unsplash.Core.Extensions;
using Unsplash.Core.Sources;
using Console = Colorful.Console;

namespace Unsplash.Desktop
{
    internal static class SettingsHelper
    {
        public static JsonSerializerSettings JsonSettings { get; } =
            new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};

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

        private static UnsplashSource GenerateUnsplashSource()
        {
            var menu = ShowEnumMenu<UnsplashSourceType>();
            Console.WriteLine(menu);

            return menu switch
            {
                UnsplashSourceType.Random => new RandomUnsplashSource(),
                UnsplashSourceType.Category => new CategoryUnsplashSource(ShowEnumMenu<Categories>().ToString()),
                UnsplashSourceType.Collection => new CollectionUnsplashSource(
                    Questions.AskQuestion("Please enter a collection ID: "),
                    Questions.AskBoolQuestion("Is this collection curated? ")),
                UnsplashSourceType.SearchTerm => new SearchtermUnsplashSource(
                    Questions.AskQuestion("Specify the search terms (Can be separated by a comma): ")),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static void SaveSettings(Settings settings)
        {
            File.WriteAllText(Environment.ExpandEnvironmentVariables("%USERPROFILE%\\Unsplash.desktop"),
                JsonConvert.SerializeObject(settings, Formatting.Indented, JsonSettings));
        }

        public static Settings GenerateSettings()
        {
            return new Settings
            {
                ImageWidth = Questions.AskIntMessage("> Enter the desired image width: ", Color.Gray),
                ImageHeight = Questions.AskIntMessage("> Enter the desired image height: ", Color.Gray),
                WallpaperDisplayStyle = ShowEnumMenu<WallpaperDisplayStyle>(),
                Source = GenerateUnsplashSource(),
                Interval = new TimeSpan(0, 0,
                    Questions.AskIntMessage("How often the wallpaper should be updated (In minutes)? "), 0, 0)
            };
        }

        public static Settings LoadSettings()
        {
            try
            {
                return
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
                Environment.Exit(-1);
                return null;
            }
        }
    }
}