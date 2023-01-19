using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Structing.Core;
using Anf.Core;
using System;

namespace Anf.Web
{
    public class Program
    {
        internal static readonly IEnumerable<IModuleEntry> modules = new ModuleCollection
        {
            new WebModuleEntry(),
            new CoreModuleEntry()
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
                .UseServiceProviderFactory(
                    new DryIocServiceProviderFactory(new Container(rules=> WithMyRules(rules))))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup(new ModuleCollection(modules));
                });
    }
}
