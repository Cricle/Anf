using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

namespace Anf.AzureFunc
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var host = new HostBuilder()
                .ConfigureServices((s,ctx) =>
                {
                    Startup.AddServices(ctx, s.Configuration);
                })
                .ConfigureAppConfiguration(x =>
                {
                    x.AddEnvironmentVariables();
                    var keyVaultEndpoint = new Uri("https://anfwebvault.vault.azure.net/");
                    x.AddAzureKeyVault(
                    keyVaultEndpoint,
                    new DefaultAzureCredential());
                    //x.AddUserSecrets(typeof(Program).Assembly);
                })
                .ConfigureFunctionsWorkerDefaults()
                .Build();

            host.Run();
        }
    }
}
