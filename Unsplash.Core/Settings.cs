using System;
using System.IO;
using Newtonsoft.Json;

namespace Unsplash.Core
{
    public class Settings
    {
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public Wallpaper.Style WallpaperStyle { get; set; }

        public void Save()
        {
            File.WriteAllText(Environment.ExpandEnvironmentVariables("%USERPROFILE%\\Unsplash.desktop"),
                JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}