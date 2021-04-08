using Avalonia;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Kw.Comic.Avalon
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Debug.WriteLine(e.Exception);
            e.SetObserved();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
#if RENDER_D2D
                .UseWin32()
                .UseDirect2D1()
#else
                .UsePlatformDetect()
#endif
                .LogToTrace();
    }
}
