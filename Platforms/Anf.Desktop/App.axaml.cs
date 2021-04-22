using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Themes.Fluent;
using Anf.Desktop.Services;
using Anf.Desktop.Views;
using Anf.Easy;
using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Anf.Services;
using Anf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using Avalonia.Input;
using System.Threading.Tasks;
using Anf.Desktop.ViewModels;
using Anf.Platform.Services;
using Anf.Desktop.Models;
using Anf.Platform;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Ao.SavableConfig;
using Microsoft.Extensions.FileProviders;

namespace Anf.Desktop
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            InitServices();
            HandleException();
        }
        private void HandleException()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var exser = AppEngine.GetRequiredService<ExceptionService>();
            exser.Exception = e.Exception;
            var logger = AppEngine.GetLogger<App>();
            logger.LogError(e.Exception, sender?.ToString() ?? string.Empty);
            e.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exser = AppEngine.GetRequiredService<ExceptionService>();
            var ex = e.ExceptionObject as Exception;
            var logger = AppEngine.GetLogger<App>();
            logger.LogError(ex, sender?.ToString() ?? string.Empty);
            exser.Exception = ex;
        }

        private void InitServices()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            AppEngine.Reset();
            AppEngine.AddServices(NetworkAdapterTypes.WebRequest);
            //var store = new GzipFileStoreService(new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, XComicConst.CacheFolderName)), MD5AddressToFileNameProvider.Instance);
            var store = FileStoreService.FromMd5Default(Path.Combine(basePath, XComicConst.CacheFolderName));
            var hp = new Lazy<HomePage>(() => new HomePage());
            var cv = new Lazy<ComicView>(() => new ComicView());
            var bv = new Lazy<BookshelfView>(() => new BookshelfView());
            var va = new ViewActiver
            {
                [typeof(HomePage)] = () => hp.Value,
                [typeof(ComicView)] = () => cv.Value,
                [typeof(BookshelfView)] = () => bv.Value
            };
            var nav = new MainNavigationService(new Border(), va);
            AppEngine.Services.AddSingleton<IViewActiver>(va);
            AppEngine.Services.AddSingleton<ThemeService>();
            AppEngine.Services.AddSingleton<TitleService>();
            AppEngine.Services.AddSingleton<INavigationService>(nav);
            AppEngine.Services.AddSingleton<IComicTurnPageService>(nav);
            AppEngine.Services.AddSingleton(nav);
            AppEngine.Services.AddSingleton(store);
            AppEngine.Services.AddSingleton<IComicSaver>(store);
            AppEngine.Services.AddSingleton<IStoreService>(store);
            AppEngine.Services.AddSingleton<IPlatformService, PlatformService>();
            AppEngine.Services.AddSingleton<IStreamImageConverter<Bitmap>, StreamImageConverter>();
            AppEngine.Services.AddSingleton<IResourceFactoryCreator<Bitmap>, AResourceCreatorFactory>();
            AppEngine.Services.AddSingleton<ExceptionService>();
            var storeSer = new AvalonComicStoreService(new DirectoryInfo(Path.Combine(basePath, XComicConst.CacheFolderName, XComicConst.StoreFolderName)));
            AppEngine.Services.AddSingleton(storeSer);
            AppEngine.Services.AddSingleton<IObservableCollectionFactory>(new AvaloniaObservableCollectionFactory());
            AppEngine.Services.AddSingleton<ComicStoreService<AvalonComicStoreBox>>(storeSer);
            AppEngine.Services.AddSingleton(HistoryService.FromFile(Path.Combine(basePath, HistoryService.HistoryFileName)));
            AppEngine.Services.AddScoped<IComicVisiting<Bitmap>, StoreComicVisiting<Bitmap>>();
            AppEngine.Services.AddScoped<StoreComicVisiting<Bitmap>>();
            AppEngine.Services.AddSingleton<AnfSetting>();
            var configRoot = BuildConfiguration();
            AppEngine.Services.AddSingleton(configRoot);
            AppEngine.Services.AddSingleton<IConfiguration>(configRoot);
            AppEngine.Services.AddSingleton<IConfigurationRoot>(configRoot);
            AppEngine.Services.AddLogging(x => x.ClearProviders().AddNLog("NLog.config"));
        }

        private SavableConfigurationRoot BuildConfiguration()
        {
            var configBuilder = new SavableConfiurationBuilder();
            configBuilder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
            configBuilder.AddJsonFile(XComicConst.SettingFileFolder,true,true);
            return configBuilder.Build();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                AppEngine.Services.AddSingleton(desktop);
                AppEngine.Services.AddSingleton<MainWindow>();
                var themeSer = AppEngine.GetRequiredService<ThemeService>();
                var nav = AppEngine.GetRequiredService<MainNavigationService>();
                var mainWin = AppEngine.GetRequiredService<MainWindow>();
                desktop.MainWindow = mainWin;
                //nav.Navigate(new VisitingView());
                nav.Navigate<HomePage>();
                var titleSer = AppEngine.GetRequiredService<TitleService>();
                titleSer.Bind(mainWin);
                mainWin.KeyDown += OnMainWinKeyDown;
                titleSer.CreateControls();
            }
            base.OnFrameworkInitializationCompleted();
        }

        private void OnMainWinKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyModifiers & KeyModifiers.Alt) != 0 &&
                (e.KeyModifiers & KeyModifiers.Shift) != 0 &&
                e.Key == Key.F12)
            {
                GC.Collect();
            }
        }
    }
}
