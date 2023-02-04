using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Anf.Desktop.Services;
using Anf.Desktop.Views;
using Anf.Easy;
using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Anf.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Avalonia.Input;
using Anf.Platform.Services;
using Anf.Platform;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Ao.SavableConfig;
using Ao.SavableConfig.Saver;
using Ao.SavableConfig.Binder;
using Anf.Desktop.Settings;
using Anf.Engine;
using Anf.Platform.Books;
using Anf.Platform.Models.Impl;
using Anf.Platform.Services.Impl;
using Anf.Platform.Engines;
using Anf.KnowEngines;

namespace Anf.Desktop
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            InitServices();
            GlobalExceptionHelper.Enable = true;
        }

        public static string Workstation { get; } = AppDomain.CurrentDomain.BaseDirectory;

        private void InitServices()
        {
            AppEngine.Reset();
            AppEngine.AddServices(NetworkAdapterTypes.WebRequest);
            //var store = new GzipFileStoreService(new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, XComicConst.CacheFolderName)), MD5AddressToFileNameProvider.Instance);
            var store = FileStoreService.FromMd5Default(Path.Combine(Workstation, XComicConst.CacheFolderName));
            var hp = new Lazy<HomePage>(() => new HomePage());
            var cv = new Lazy<ComicView>(() => new ComicView());
            var bv = new Lazy<BookshelfView>(() => new BookshelfView());
            var va = new ViewActiver<IControl>
            {
                [typeof(HomePage)] = () => hp.Value,
                [typeof(ComicView)] = () => cv.Value,
                [typeof(BookshelfView)] = () => bv.Value
            };
            var nav = new MainNavigationService(new Border(), va);
            AppEngine.Services.AddSingleton(x => new BookManager(new DirectoryInfo(Path.Combine(Workstation, XComicConst.BookFolderName))));
            AppEngine.Services.AddSingleton<IViewActiver<IControl>>(va);
            AppEngine.Services.AddSingleton<ThemeService>();
            AppEngine.Services.AddSingleton<TitleService>();
            AppEngine.Services.AddSingleton<IComicTurnPageService>(nav);
            AppEngine.Services.AddSingleton(nav);
            AppEngine.Services.AddScoped<RemoteEngine>();
            AppEngine.Services.AddSingleton<IComicSaver>(store);
            AppEngine.Services.AddSingleton<IStoreService>(store);
            AppEngine.Services.AddSingleton<IPlatformService, PlatformService>();
            AppEngine.Services.AddSingleton<IStreamImageConverter<Bitmap>, StreamImageConverter>();
            AppEngine.Services.AddSingleton<IResourceFactoryCreator<Bitmap>, PlatformResourceCreatorFactory<Bitmap, Bitmap>>();
            AppEngine.Services.AddSingleton<ExceptionService>();
            var storeSer = new WithImageComicStoreService<Bitmap, Bitmap>(new DirectoryInfo(Path.Combine(Workstation, XComicConst.CacheFolderName, XComicConst.StoreFolderName)));
            AppEngine.Services.AddSingleton(storeSer);
            AppEngine.Services.AddSingleton<IObservableCollectionFactory>(new AvaloniaObservableCollectionFactory());
            AppEngine.Services.AddSingleton<ComicStoreService<WithImageComicStoreBox<Bitmap, Bitmap>>>(storeSer);
            AppEngine.Services.AddSingleton(HistoryService.FromFile(Path.Combine(Workstation, HistoryService.HistoryFileName)));
            AppEngine.Services.AddSingleton<ProposalEngine>();
            AppEngine.Services.AddScoped<IComicVisiting<Bitmap>, DesktopStoreComicVisiting>();
            AppEngine.Services.AddScoped<StoreComicVisiting<Bitmap>>();

            var configRoot = BuildConfiguration();
            AppEngine.Services.AddSingleton(CreateSettings);
            AppEngine.Services.AddSingleton(configRoot);
            AppEngine.Services.AddSingleton<IConfiguration>(configRoot);
            AppEngine.Services.AddSingleton<IConfigurationRoot>(configRoot);
            AppEngine.Services.AddLogging(x => 
            {
                x.ClearProviders();
                x.AddNLog("NLog.config");
            });
        }
        private AnfSettings CreateSettings(IServiceProvider provider)
        {
            var root = provider.GetRequiredService<SavableConfigurationRoot>();
            var instType = ProxyHelper.Default.CreateComplexProxy<AnfSettings>();
            var inst = (AnfSettings)instType.Build(root);
            _ = root.BindTwoWay(inst, JsonChangeTransferCondition.Instance);
            return inst;
        }

        private SavableConfigurationRoot BuildConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Workstation);
            configBuilder.AddJsonFile(XComicConst.SettingFileFolder, true, true);
            return configBuilder.BuildSavable();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                AppEngine.Services.AddSingleton(desktop);
                AppEngine.Services.AddSingleton<MainWindow>();
                AppEngine.Provider.UseKnowEngines();
                var nav = AppEngine.GetRequiredService<MainNavigationService>();
                var mainWin = AppEngine.GetRequiredService<MainWindow>();
                var titleSer = AppEngine.GetRequiredService<TitleService>();
                _ = AppEngine.GetRequiredService<AnfSettings>();
                mainWin.Icon = new WindowIcon("Anf.ico");
                desktop.MainWindow = mainWin;
                titleSer.Bind(mainWin);
                titleSer.CreateControls();
                mainWin.KeyDown += OnMainWinKeyDown;

                //nav.Navigate(new VisitingView());
                nav.Navigate<HomePage>();
                AppEngine.GetRequiredService<ThemeService>();
                mainWin.RunInitAll();
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
