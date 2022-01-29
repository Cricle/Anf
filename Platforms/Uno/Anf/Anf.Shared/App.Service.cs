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
using Org.BouncyCastle.Crmf;
using Windows.UI.Xaml.Media;
using Anf.Settings;
using Windows.UI.Xaml;
using Windows.Storage;

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
            AppEngine.Services.AddSingleton<IStreamImageConverter<ImageSource>, StreamImageConverter>();
            AppEngine.Services.AddSingleton<IResourceFactoryCreator<ImageSource>, PlatformResourceCreatorFactory<ImageSource, ImageSource>>();
            AppEngine.Services.AddSingleton<ExceptionService>();
            var storeSer = new WithImageComicStoreService<ImageSource, ImageSource>(new DirectoryInfo(Path.Combine(Workstation, XComicConst.CacheFolderName, XComicConst.StoreFolderName)));
            AppEngine.Services.AddSingleton(storeSer);
            AppEngine.Services.AddSingleton<IObservableCollectionFactory>(new UnoObservableCollectionFactory());
            AppEngine.Services.AddSingleton<ComicStoreService<WithImageComicStoreBox<ImageSource, ImageSource>>>(storeSer);
            AppEngine.Services.AddSingleton(HistoryService.FromFile(Path.Combine(Workstation, HistoryService.HistoryFileName)));
            AppEngine.Services.AddSingleton<ProposalEngine>();
            AppEngine.Services.AddScoped<IComicVisiting<ImageSource>, UnoStoreComicVisiting>();
            AppEngine.Services.AddScoped<StoreComicVisiting<ImageSource>>();

            var configRoot = BuildConfiguration();
            AppEngine.Services.AddSingleton(CreateSettings);
            AppEngine.Services.AddSingleton(configRoot);
            AppEngine.Services.AddSingleton<IConfiguration>(configRoot);
            AppEngine.Services.AddSingleton<IConfigurationRoot>(configRoot);
            AppEngine.Services.AddLogging(x =>
            {
                x.ClearProviders();
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
    }
}
