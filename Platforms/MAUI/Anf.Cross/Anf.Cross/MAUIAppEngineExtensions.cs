using Anf.Cross.Services;
using Anf.Cross.Settings;
using Anf.Cross.ViewModels;
using Anf.Easy;
using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Anf.Engine;
using Anf.Platform;
using Anf.Platform.Models.Impl;
using Anf.Platform.Services;
using Anf.Platform.Services.Impl;
using Anf.Services;
using Ao.SavableConfig;
using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Saver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using System.IO;

namespace Anf.Cross
{
    public static class MAUIAppEngineExtensions
    {
        private static readonly string Workstation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static void AddViewModels(this IServiceCollection services)
        {
            services.AddScoped<MAUIHomeViewModel>();
        }

        public static void AddMAUIService(this IServiceCollection services)
        {
            var store = FileStoreService.FromMd5Default(Path.Combine(Workstation, XComicConst.CacheFolderName));
            
            services.AddSingleton<ProposalEngine>();
            services.AddSingleton<IComicSaver>(store);
            services.AddSingleton<IStoreService>(store);
            services.AddSingleton<IPlatformService, PlatformService>();
            services.AddScoped<IComicVisiting<ImageSource>, ComicVisiting<ImageSource>>();
            services.AddSingleton<IStreamImageConverter<ImageSource>, StreamImageConverter>();
            services.AddSingleton<IStreamImageConverter<ImageResource>, StreamResourceConverter>();
            services.AddSingleton<IResourceFactoryCreator<ImageSource>, PlatformResourceCreatorFactory<ImageResource, ImageSource>>();
            services.AddSingleton<ExceptionService>();
            services.AddScoped<StoreComicVisiting<ImageSource>>();

            var storeSer = new WithImageComicStoreService<ImageResource, ImageSource>(new DirectoryInfo(Path.Combine(Workstation, XComicConst.CacheFolderName, XComicConst.StoreFolderName)));
            services.AddSingleton(storeSer);
            services.AddSingleton<IObservableCollectionFactory>(new DefaultObservableCollectionFactory());
            services.AddSingleton<ComicStoreService<WithImageComicStoreBox<ImageResource, ImageSource>>>(storeSer);

            var configRoot = BuildConfiguration();
            services.AddSingleton(CreateSettings);
            services.AddSingleton(configRoot);
            services.AddSingleton<IConfiguration>(configRoot);
            services.AddSingleton<IConfigurationRoot>(configRoot);
            //LogManager.Configuration = new XmlLoggingConfiguration(XmlReader.Create(logXml));
            //services.AddLogging(x => x.ClearProviders().AddNLog());

        }


        private static AnfSettings CreateSettings(IServiceProvider provider)
        {
            var root = provider.GetRequiredService<SavableConfigurationRoot>();
            var instType = ProxyHelper.Default.CreateComplexProxy<AnfSettings>(true);
            var inst = (AnfSettings)instType.Build(root);
            var disposable = root.BindTwoWay(inst, JsonChangeTransferCondition.Instance);
            return inst;
        }

        private static SavableConfigurationRoot BuildConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(Workstation);
            configBuilder.AddJsonFile(Path.Combine(Workstation, XComicConst.SettingFileName), true, true);
            return configBuilder.BuildSavable();
        }
    }
}
