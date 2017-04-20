using System;
using System.IO;
using Newtonsoft.Json;

namespace Unsplash.Core
{
    public class Settings
    {
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public TimeSpan Interval { get; set; }
        public UnsplashSource Source { get; set; }
        public WallpaperDisplayStyle WallpaperDisplayStyle { get; set; }


        [JsonIgnore]
        public string Resolution => $"{ImageWidth}x{ImageHeight}";

        public void Save()
        {
            File.WriteAllText(Environment.ExpandEnvironmentVariables("%USERPROFILE%\\Unsplash.desktop"),
                JsonConvert.SerializeObject(this, Formatting.Indented, Unsplash.JsonSettings));
        }
    }
}