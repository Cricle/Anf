using Anf.Easy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;

namespace Anf.Cross
{
    public class Startup : IStartup
    {
        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .ConfigureServices(s =>
                {
                    AppEngine.Reset();
                    AddCore();
                })
                .UseFormsCompatibility()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
        }
        private static void AddCore()
        {
            AppEngine.Services.AddViewModels();
            AppEngine.Services.AddMAUIService();
            AppEngine.AddServices(NetworkAdapterTypes.WebRequest);

        }
    }
}