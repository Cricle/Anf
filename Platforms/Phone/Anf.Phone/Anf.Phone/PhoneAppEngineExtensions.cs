using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Anf;
using Anf.Easy;
using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Anf.Engine;
using Anf.Phone.Models;
using Anf.Phone.Services;
using Anf.Phone.Settings;
using Anf.Platform;
using Anf.Platform.Services;
using Anf.Services;
using Ao.SavableConfig;
using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Saver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using Xamarin.Forms;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PhoneAppEngineExtensions
    {
        private static readonly string Workstation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static void AddPhoneService(this IServiceCollection services,Stream logXml)
        {
            var store = FileStoreService.FromMd5Default(Path.Combine(Workstation, XComicConst.CacheFolderName));

            services.AddSingleton<ProposalEngine>();
            services.AddSingleton<IComicSaver>(store);
            services.AddSingleton<IStoreService>(store);
            services.AddSingleton<IPlatformService, PlatformService>();
            services.AddScoped<IComicVisiting<ImageSource>,ComicVisiting<ImageSource>>();
            services.AddSingleton<IStreamImageConverter<ImageSource>, StreamImageConverter>();
            services.AddSingleton<IResourceFactoryCreator<ImageSource>, PlatformResourceCreatorFactory<ImageSource>>();
            services.AddSingleton<ExceptionService>();

            var storeSer = new PhoneComicStoreService(new DirectoryInfo(Path.Combine(Workstation, XComicConst.CacheFolderName, XComicConst.StoreFolderName)));
            services.AddSingleton(storeSer);
            services.AddSingleton<IObservableCollectionFactory>(new DefaultObservableCollectionFactory());
            services.AddSingleton<ComicStoreService<PhoneComicStoreBox>>(storeSer);

            var configRoot = BuildConfiguration();
            services.AddSingleton(CreateSettings);
            services.AddSingleton(configRoot);
            services.AddSingleton<IConfiguration>(configRoot);
            services.AddSingleton<IConfigurationRoot>(configRoot);            
            LogManager.Configuration= new XmlLoggingConfiguration(XmlReader.Create(logXml));
            services.AddLogging(x => x.ClearProviders().AddNLog());

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
            var configBuilder = new SavableConfiurationBuilder();
            configBuilder.SetBasePath(Workstation);
            configBuilder.AddJsonFile(Path.Combine(Workstation, XComicConst.SettingFileName), true, true);
            return configBuilder.Build();
        }
    }
}
