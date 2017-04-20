using System.ComponentModel;

namespace Unsplash.Core
{
    public enum WallpaperDisplayStyle
    {
        Fill,
        Fit,
        Stretch,
        Tile,
        Center,
        [Description("Windows 8 or newer")] Span
    }
}