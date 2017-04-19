using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Unsplash.Core
{
    public sealed class Wallpaper
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style
        {
            Fit,
            [Description("Windows 8 or newer")] Span,
            Stretch,
            Tile,
            Center,
            Fill
        }

        public static void Set(Uri uri, Style style)
        {
            try
            {
                var s = new WebClient().OpenRead(uri.ToString());

                var img = Image.FromStream(s);
                var tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
                img.Save(tempPath, ImageFormat.Bmp);

                var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

                if (style == Style.Fill)
                {
                    key.SetValue(@"WallpaperStyle", 10.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
                if (style == Style.Fit)
                {
                    key.SetValue(@"WallpaperStyle", 6.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
                if (style == Style.Span) // Windows 8 or newer only!
                {
                    key.SetValue(@"WallpaperStyle", 22.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
                if (style == Style.Stretch)
                {
                    key.SetValue(@"WallpaperStyle", 2.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
                if (style == Style.Tile)
                {
                    key.SetValue(@"WallpaperStyle", 0.ToString());
                    key.SetValue(@"TileWallpaper", 1.ToString());
                }
                if (style == Style.Center)
                {
                    key.SetValue(@"WallpaperStyle", 0.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }
                SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0,
                    tempPath,
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
            catch
            {
                //Supress
            }
        }
    }
}