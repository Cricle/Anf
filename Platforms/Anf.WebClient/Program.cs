using Anf.Easy.Visiting;
using Anf.Engine;
using Anf.Networks;
using Anf.Platform;
using Anf.Platform.Services;
using Anf.WebClient.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Anf.KnowEngines;

namespace Anf.WebClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            AppEngine.Reset(builder.Services);
            builder.RootComponents.Add<App>("#app");

            AppEngine.AddServices();
            builder.Services.AddKnowEngines();

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<INetworkAdapter, ProxyNetworkAdapter>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton<ExceptionService>();
            builder.Services.AddSingleton<IObservableCollectionFactory>(new DefaultObservableCollectionFactory());
            builder.Services.AddScoped<IComicVisiting<Stream>, StoreComicVisiting<Stream>>();
            builder.Services.AddSingleton<ProposalEngine>();
            builder.Services.AddSingleton<IStreamImageConverter<Stream>, StreamImageConverter>();

            var app = builder.Build();
            AppEngine.UseProvider(app.Services);
            app.Services.UseKnowEngines();
            await app.RunAsync();
        }
    }
}
