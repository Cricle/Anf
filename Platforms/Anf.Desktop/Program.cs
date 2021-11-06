using Avalonia;
using System.Threading.Tasks;
using System.Diagnostics;
using Anf.KnowEngines;
using Anf.Networks;
using Microsoft.IO;
using System.Net.Http;
using System.IO;
using System;
using Anf.Desktop.Services;
using Anf.Platform.Services;

namespace Anf.Desktop
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            try
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exSer = AppEngine.GetRequiredService<ExceptionService>();
            exSer.Exception = e.ExceptionObject as Exception;
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var exSer = AppEngine.GetRequiredService<ExceptionService>();
            exSer.Exception = e.Exception;
            e.SetObserved();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
#if RENDERSYSTEM_D2D
                .UseWin32()
                .UseDirect2D1()
#else
                .UsePlatformDetect()
#endif
                .LogToTrace();
    }
}
