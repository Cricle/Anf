using Anf.Easy;
using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Anf.Services;
using Anf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anf.Platform.Services;
using Anf.Platform;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Ao.SavableConfig;
using Microsoft.Extensions.FileProviders;
using Ao.SavableConfig.Saver;
using Ao.SavableConfig.Binder;
using Anf.Engine;
using Anf.Platform.Books;
using Microsoft.IO;
using Anf.Platform.Models.Impl;
using Anf.Platform.Services.Impl;
using Anf.Platform.Engines;
using Anf.KnowEngines;
using Windows.UI.Xaml.Media;
using Anf.Settings;
using Windows.UI.Xaml;
using Windows.Storage;
using Anf.Views;
using Anf.Platform.Settings;

namespace Anf
{
    public partial class App
    {
        public static string Workstation { get; } = ApplicationData.Current.LocalFolder.Path;

        private void InitServices()
        {
            AppEngine.Reset();
            AppEngine.AddServices(NetworkAdapterTypes.WebRequest);
            //var store = new GzipFileStoreService(new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, XComicConst.CacheFolderName)), MD5AddressToFileNameProvider.Instance);
            var store = FileStoreService.FromMd5Default(Path.Combine(Workstation, XComicConst.CacheFolderName));
            AppEngine.Services.AddSingleton(x => new BookManager(new DirectoryInfo(Path.Combine(Workstation, XComicConst.BookFolderName)), x.GetRequiredService<RecyclableMemoryStreamManager>()));
            //AppEngine.Services.AddSingleton<IViewActiver<IControl>>(va);
            AppEngine.Services.AddScoped<RemoteEngine>();
            AppEngine.Services.AddSingleton(new UnoThemeService());
            AppEngine.Services.AddSingleton<IComicSaver>(store);
            AppEngine.Services.AddSingleton<IStoreService>(store);
            AppEngine.Services.AddSingleton<IPlatformService, PlatformService>();
            AppEngine.Services.AddSingleton<IStreamImageConverter<ImageBox>, StreamImageConverter>();
            AppEngine.Services.AddSingleton<IResourceFactoryCreator<ImageBox>>(new PlatformResourceCreatorFactory<ImageBox, ImageBox>
            {
                EnableCache = true,
                StoreFetchSettings = StoreFetchSettings.DefaultNoDisposeStream.Clone()
            });
            AppEngine.Services.AddSingleton<ExceptionService>();
            var storeSer = new WithImageComicStoreService<ImageBox, ImageBox>(new DirectoryInfo(Path.Combine(Workstation, XComicConst.CacheFolderName, XComicConst.StoreFolderName)));
            AppEngine.Services.AddSingleton(storeSer);
            AppEngine.Services.AddSingleton<IObservableCollectionFactory>(new UnoObservableCollectionFactory());
            AppEngine.Services.AddSingleton<ComicStoreService<WithImageComicStoreBox<ImageBox, ImageBox>>>(storeSer);
            AppEngine.Services.AddSingleton(HistoryService.FromFile(Path.Combine(Workstation, HistoryService.HistoryFileName)));
            AppEngine.Services.AddSingleton<ProposalEngine>();
            AppEngine.Services.AddScoped<IComicVisiting<ImageBox>, UnoStoreComicVisiting>();
            AppEngine.Services.AddScoped<StoreComicVisiting<ImageBox>>();

            var configRoot = BuildConfiguration();
            AppEngine.Services.AddSingleton(CreateSettings);
            AppEngine.Services.AddSingleton(configRoot);
            AppEngine.Services.AddSingleton<IConfiguration>(configRoot);
            AppEngine.Services.AddSingleton<IConfigurationRoot>(configRoot);
            AppEngine.Services.AddSingleton(new UnoRuntime());
            AppEngine.Services.AddSingleton<UnoNavigationService>();
            AppEngine.Services.AddSingleton<IComicTurnPageService>(x => x.GetRequiredService<UnoNavigationService>());
            AppEngine.Services.AddLogging(x =>
            {
                x.ClearProviders();
#if HAS_UNO_SKIA_WPF
                x.AddConsole();
#endif
            });

            AppEngine.Services.AddSingleton<UnoHomeViewModel>();
            var appBarSer = new AppBarService();
            appBarSer.GetAsDefault()
                .Rights.Add(new DefaultControlView());
            AppEngine.Services.AddSingleton(appBarSer);
            AppEngine.Services.AddSingleton(new UnoTtileService());
        }
        private AnfSettings CreateSettings(IServiceProvider provider)
        {
            return new AnfSettings
            {
                Performace = new PerformaceSettings(),
                DotShowException = true,
                Reading = new ReadingSettings(),
                Startup = new StartupSettings
                {
                    DisplayProposalCount = 10
                },
                Window = new WindowSettings(),
            };
            //var root = provider.GetRequiredService<SavableConfigurationRoot>();
            //var instType = ProxyHelper.Default.CreateComplexProxy<AnfSettings>();
            //var inst = (AnfSettings)instType.Build(root);
            //_ = root.BindTwoWay(inst, JsonChangeTransferCondition.Instance);
            //return inst;
        }

        private IConfigurationRoot BuildConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Workstation);
            configBuilder.AddJsonFile(XComicConst.SettingFileFolder, true, false);
            return configBuilder.Build();
        }
    }
}
