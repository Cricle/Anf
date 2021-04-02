using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using Avalonia.OpenGL;

namespace Kw.Comic.Avalon
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
#if NET472
                .UseWin32()
                .UseDirect2D1()
#else
                .UsePlatformDetect()
#endif
                .LogToTrace();
    }
}
