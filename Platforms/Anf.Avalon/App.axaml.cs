using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Themes.Fluent;
using Anf.Avalon.Services;
using Anf.Avalon.Views;
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
using Anf.Avalon.ViewModels;
using Anf.Platform.Services;

namespace Anf.Avalon
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
            e.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exser = AppEngine.GetRequiredService<ExceptionService>();
            exser.Exception = e.ExceptionObject as Exception;
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
            AppEngine.Services.AddSingleton(new ComicStoreService(new DirectoryInfo(Path.Combine(basePath,XComicConst.StoreFolderName))));
            AppEngine.Services.AddSingleton(HistoryService.FromFile(Path.Combine(basePath, HistoryService.HistoryFileName)));
            AppEngine.Services.AddScoped<IComicVisiting<Bitmap>, ComicVisiting<Bitmap>>();

            var style = Styles.Where(x => x is FluentTheme).FirstOrDefault() as FluentTheme;
            AppEngine.Services.AddSingleton(style);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                AppEngine.Services.AddSingleton(desktop);
                AppEngine.Services.AddSingleton<MainWindow>();
                AppEngine.GetRequiredService<ThemeService>();
                var nav = AppEngine.GetRequiredService<MainNavigationService>();
                var mainWin = AppEngine.GetRequiredService<MainWindow>();
                desktop.MainWindow =mainWin;
                //nav.Navigate(new VisitingView());
                nav.Navigate<HomePage>();
                AppEngine.GetRequiredService<TitleService>().Bind(mainWin);
                mainWin.KeyDown += OnMainWinKeyDown;

                //var vc = new VisitingControlView { DataContext = new AvalonVisitingViewModel() };
                //var titleService = AppEngine.GetRequiredService<TitleService>();
                //titleService.LeftControls.Add(vc);

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
