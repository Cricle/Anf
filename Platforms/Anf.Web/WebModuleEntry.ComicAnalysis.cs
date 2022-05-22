using Anf.Easy;
using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Anf.Engine;
using Anf.KnowEngines;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Anf.Web
{
    internal partial class WebModuleEntry
    {
        public WebModuleEntry AddComicAnalysis(IServiceCollection services)
        {
            AppEngine.Reset(services);
            AppEngine.AddServices(NetworkAdapterTypes.HttpClient);
            services.AddSingleton<ProposalEngine>();
            services.AddScoped<IComicVisiting<Stream>, WebComicVisiting>();
            services.AddKnowEngines();
            return this;
        }
        public WebModuleEntry AddAzureStore(IServiceCollection services,IConfiguration config)
        {
            services.AddSingleton(provider => new AzureStoreService(
                provider.GetRequiredService<BlobServiceClient>(), MD5AddressToFileNameProvider.Instance, 512));
            services.AddSingleton<IComicSaver>(provider => provider.GetRequiredService<AzureStoreService>());
            services.AddSingleton<IStoreService>(provider => provider.GetRequiredService<AzureStoreService>());
            services.AddSingleton<IResourceFactoryCreator<Stream>, WebResourceFactoryCreator>();
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(config["AzureStoreBlobblob"], preferMsi: true);
                builder.AddQueueServiceClient(config["AzureStoreBlobqueue"], preferMsi: true);
            });
            return this;
        }
    }
}
