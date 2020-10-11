using System;
using Unsplash.Core.Enums;

namespace Unsplash.Core
{
    public class Settings
    {
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public TimeSpan Interval { get; set; }
        public UnsplashSource Source { get; set; }
        public WallpaperDisplayStyle WallpaperDisplayStyle { get; set; }
        
        public string GetResolution() => $"{ImageWidth}x{ImageHeight}";
    }
}