using Kw.Core;
using Microsoft.Extensions.Configuration;

namespace Kw.Comic.Blazor.Server
{
    internal static class RegisteContextExtensions
    {
        private const string ConfigurationKey = "Kw.Comic.Configuration";

        public static IConfiguration GetConfiguaration(this IRegisteContext context)
        {
            return (IConfiguration)context.Features[ConfigurationKey];
        }
        public static void SetConfiguaration(this IRegisteContext context, IConfiguration configuration)
        {
            context.Features[ConfigurationKey] = configuration;
        }
    }
}
