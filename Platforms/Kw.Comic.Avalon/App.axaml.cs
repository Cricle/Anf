using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Kw.Comic.Avalon.Views;
using Kw.Comic.Engine.Easy;
using Kw.Comic.Engine.Easy.Visiting;
using Kw.Comic.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kw.Comic.Avalon
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            AppEngine.Reset();
            AppEngine.AddServices(NetworkAdapterTypes.HttpClient);
            AppEngine.Services.AddSingleton<IComicVisiting<Bitmap>, ComicVisiting<Bitmap>>();
            AppEngine.Services.AddSingleton<IResourceFactoryCreator<Bitmap>, AResourceCreatorFactory>();
            
            AppEngine.Services.AddScoped<HomeViewModel>();
            AppEngine.Services.AddScoped<BookshelfViewModel>();
            //AppEngine.Services.AddScoped<HomeViewModel>();
        }
        
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = AppEngine.GetRequiredService<HomeViewModel> (),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
