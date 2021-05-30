using GalaSoft.MvvmLight;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Cross.Services
{
    public class ThemeService: ObservableObject,IDisposable
    {
        public ThemeService()
        {
            appFactory = new Lazy<Application>(GetApplication);            
        }

        private void OnCurrentRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Theme));
        }

        public void Dispose()
        {
            App.RequestedThemeChanged -= OnCurrentRequestedThemeChanged;
        }
        private readonly Lazy<Application> appFactory;
        public Application App => appFactory.Value;

        public OSAppTheme Theme
        {
            get => App.RequestedTheme;
            set
            {
                App.UserAppTheme = value;
                RaisePropertyChanged();
            }
        }

        private Application GetApplication()
        {
            var app= Application.Current;
            if (app is null)
            {
                throw new InvalidOperationException("Application.Current is null!");
            }
            app.RequestedThemeChanged += OnCurrentRequestedThemeChanged;
            return app;
        }
    }
}
