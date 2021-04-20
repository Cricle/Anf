using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Themes.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Services
{
    internal class ThemeService
    {

        public ThemeService(FluentTheme fluentTheme, IClassicDesktopStyleApplicationLifetime app,
            MainWindow window)
        {
            FluentTheme = fluentTheme;
            App = app;
            MainWindow = window;
            InitWin();
        }
        public WindowTransparencyLevel TransparencyLevel
        {
            get => MainWindow.TransparencyLevelHint;
            set => MainWindow.TransparencyLevelHint = value;
        }
        public SystemDecorations SystemDecorations
        {
            get => MainWindow.SystemDecorations;
            set => MainWindow.SystemDecorations = value;
        }
        public FluentThemeMode Mode
        {
            get => FluentTheme.Mode;
            set
            {
                FluentTheme.Mode = value;
            }
        }
        public FluentTheme FluentTheme { get; }

        public IClassicDesktopStyleApplicationLifetime App { get; }

        public MainWindow MainWindow { get; }

        private void InitWin()
        {
            var win = MainWindow;
            win.ExtendClientAreaToDecorationsHint = true;
            win.ExtendClientAreaTitleBarHeightHint = -1;

            win.GetObservable(Window.WindowStateProperty)
                .Subscribe(x =>
                {
                    win.SetPseudoClasses(":maximized", x == WindowState.Maximized);
                    win.SetPseudoClasses(":fullscreen", x == WindowState.FullScreen);
                });

            win.GetObservable(Window.IsExtendedIntoWindowDecorationsProperty)
                .Subscribe(x =>
                {
                    if (!x)
                    {
                        win.SystemDecorations = SystemDecorations.Full;
                        win.TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;
                    }
                });

        }
        public void DisEnableBlur()
        {
            MainWindow.TransparencyLevelHint = WindowTransparencyLevel.None;
        }
        public void EnableBlur()
        {
            MainWindow.TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;
        }
        
    }
    public static class WindowExtensions
    {
        public static IDisposable BindDecorationMargin(this Window win,Action<Thickness> action)
        {
            return win.GetObservable(Window.WindowDecorationMarginProperty)
                 .Subscribe(action);
        }
        public static IDisposable BindDecorationMargin(this Window win,Layoutable inst)
        {
            return win.GetObservable(Window.WindowDecorationMarginProperty)
                 .Subscribe(x =>
                 {
                     inst.Margin = x;
                 });
        }
    }
}
