using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
                    x.AddUserSecrets(typeof(Program).Assembly);
                    x.AddEnvironmentVariables();
                })
                .ConfigureFunctionsWorkerDefaults()
                .Build();

            host.Run();
        }
    }
}
