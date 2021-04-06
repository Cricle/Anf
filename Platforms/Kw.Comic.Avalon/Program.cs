using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using Avalonia.OpenGL;
using System.Threading.Tasks;
using System.Diagnostics;
using Kw.Comic.Tencent;
using Kw.Comic.Engine.Networks;
using System.Net.Http;
using JavaScriptEngineSwitcher.Jint;
using System.IO;

namespace Kw.Comic.Avalon
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            //var op = new TencentComicOperator(new HttpClientAdapter(new HttpClient()), new JintJsEngine());
            //var strea = op.GetImageStreamAsync("https://manhua.acimg.cn/manhua_detail/0/03_22_21_a5b0aaab8ce403ed79975fc29ea8740ff_116284636.png/0").GetAwaiter().GetResult();
            //var sr = new StreamReader(strea).ReadToEnd();
            //op.GetPagesAsync("https://ac.qq.com/ComicView/index/id/530969/cid/43").GetAwaiter().GetResult();
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
