﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Services
{
    internal class ThemeService : ObservableObject
    {
        public ThemeService(MainWindow window)
        {
            MainWindow = window;
            InitWin();
            SwitchModel(FluentThemeMode.Dark);
        }
        private FluentThemeMode currentModel;

        public bool EnableAcrylicBlur
        {
            get => MainWindow.TransparencyLevelHint == WindowTransparencyLevel.AcrylicBlur;
            set
            {
                if (MainWindow.CheckAccess())
                {
                    MainWindow.TransparencyLevelHint = value ? WindowTransparencyLevel.AcrylicBlur : WindowTransparencyLevel.None;
                }
                else
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        MainWindow.TransparencyLevelHint = value ? WindowTransparencyLevel.AcrylicBlur : WindowTransparencyLevel.None;
                    });
                }
            }
        }

        public FluentThemeMode CurrentModel
        {
            get { return currentModel; }
            set
            {
                if (currentModel == value)
                {
                    return;
                }
                if (MainWindow.CheckAccess())
                {
                    UpdateValue();
                }
                else
                {
                    Dispatcher.UIThread.Post(UpdateValue);
                }

                void UpdateValue()
                {
                    SetProperty(ref currentModel, value);
                    SwitchModel(value);
                }
            }
        }

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
        private readonly FluentTheme lightTheme = new FluentTheme(new Uri("avares://Avalonia.Themes.Fluent/FluentLight.xaml"));
        private readonly FluentTheme darkTheme = new FluentTheme(new Uri("avares://Avalonia.Themes.Fluent/FluentDark.xaml")) { Mode = FluentThemeMode.Dark };
        public void SwitchModel(FluentThemeMode mode)
        {
            Application.Current.Styles[0]
                = mode == FluentThemeMode.Dark ? darkTheme : lightTheme;
            CurrentModel = mode;
        }

    }
    public static class WindowExtensions
    {
        public static IDisposable BindDecorationMargin(this Window win, Action<Thickness> action)
        {
            return win.GetObservable(Window.WindowDecorationMarginProperty)
                 .Subscribe(action);
        }
        public static IDisposable BindDecorationMargin(this Window win, Layoutable inst)
        {
            return win.GetObservable(Window.WindowDecorationMarginProperty)
                 .Subscribe(x =>
                 {
                     inst.Margin = x;
                 });
        }
    }
}
