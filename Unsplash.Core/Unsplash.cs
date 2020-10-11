using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.TaskScheduler;
using Task = System.Threading.Tasks.Task;

namespace Unsplash.Core
{
    public class Unsplash
    {
        private const string TaskName = "Update wallpaper";

        public Unsplash(ILogger<Unsplash> logger)
        {
            Logger = logger;
        }

        private ILogger<Unsplash> Logger { get; }

        public async Task RefreshWallpaper(Settings settings)
        {
            Logger.LogInformation("> Downloading wallpaper image...");
            await Wallpaper.Set(
                new Uri(settings.Source.BuildUrlString(settings)),
                settings.WallpaperDisplayStyle);
            Logger.LogInformation("> New wallpaper set!");
        }

        public void CreateLoopTask(Settings settings)
        {
            var executableFile = Process.GetCurrentProcess().Modules.Cast<ProcessModule>()
                .FirstOrDefault(i =>
                    i.ModuleName != null && i.ModuleName.StartsWith("Unsplash.Desktop.exe"))?.FileName;

            if (string.IsNullOrEmpty(executableFile))
            {
#if RELEASE
                Logger.LogCritical("Can't find this executable location to create the task.");
                Environment.Exit(-1);
#endif
                return;
            }

            using var ts = new TaskService();
            if (ts.RootFolder.EnumerateTasks().Any(i => i.Name == TaskName)) ts.RootFolder.DeleteTask(TaskName);

            var td = ts.NewTask();
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.RegistrationInfo.Description =
                "Changes wallpaper using unsplash. Help us @ https://github.com/redbaty/Unsplash.Desktop";
            td.Triggers.Add(new DailyTrigger
            {
                StartBoundary = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                Repetition = new RepetitionPattern(settings.Interval, TimeSpan.Zero)
            });

            td.Actions.Add(new ExecAction(executableFile, "-h"));
            ts.RootFolder.RegisterTaskDefinition(TaskName, td);
        }
    }
}