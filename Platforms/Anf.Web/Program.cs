using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Structing.Core;

namespace Anf.Web
{
    public class Program
    {
        internal static readonly IEnumerable<IModuleEntry> modules = new ModuleCollection
        {
            new WebModuleEntry()
        };

        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();
            AppEngine.UseProvider(builder.Services);
            builder.Run();
        }
        static Rules WithMyRules(Rules currentRules) => currentRules;

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        config.AddUserSecrets<Program>();

                    }
                    else
                    {
                        var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
                        config.AddAzureKeyVault(
                        keyVaultEndpoint,
                        new DefaultAzureCredential());
                    }
                })
                .UseServiceProviderFactory(
                    new DryIocServiceProviderFactory(new Container(rules=> WithMyRules(rules))))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup(new ModuleCollection(modules));
                });
    }
}
