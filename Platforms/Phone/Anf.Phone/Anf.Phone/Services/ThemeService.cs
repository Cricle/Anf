using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Anf.Phone.Services
{
    public class ThemeService: ObservableObject,IDisposable
    {
        public ThemeService()
        {
            app = Application.Current;
            if (app is null)
            {
                throw new InvalidOperationException("Application.Current is null!");
            }
            app.RequestedThemeChanged += OnCurrentRequestedThemeChanged;
        }

        private void OnCurrentRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Theme));
        }

        public void Dispose()
        {
            app.RequestedThemeChanged -= OnCurrentRequestedThemeChanged;
        }
        private readonly Application app;

        public OSAppTheme Theme
        {
            get => app.RequestedTheme;
            set
            {
                app.UserAppTheme = value;
                RaisePropertyChanged();
            }
        }


    }
}
