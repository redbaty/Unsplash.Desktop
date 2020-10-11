using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32;
using Unsplash.Core.Enums;

namespace Unsplash.Core
{
    public sealed class Wallpaper
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public static async Task Set(Uri uri, WallpaperDisplayStyle wallpaperDisplayStyle)
        {
            using var client = new HttpClient();
            var imageStream = await client.GetByteArrayAsync(uri);
          
            var imagePath = Path.GetTempFileName();
            await File.WriteAllBytesAsync(imagePath, imageStream);

            var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            if (key == null)
            {
                throw new InvalidOperationException("The registry key 'Control Panel\\Desktop' could not be found");
            }

            if (wallpaperDisplayStyle == WallpaperDisplayStyle.Fill)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (wallpaperDisplayStyle == WallpaperDisplayStyle.Fit)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (wallpaperDisplayStyle == WallpaperDisplayStyle.Span) // Windows 8 or newer only!
            {
                key.SetValue(@"WallpaperStyle", 22.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (wallpaperDisplayStyle == WallpaperDisplayStyle.Stretch)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (wallpaperDisplayStyle == WallpaperDisplayStyle.Tile)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            if (wallpaperDisplayStyle == WallpaperDisplayStyle.Center)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                imagePath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}