using Anf.WebService;
using Microsoft.Extensions.DependencyInjection;

namespace Anf.Web
{
    internal partial class WebModuleEntry
    {
        public WebModuleEntry AddAuth(IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddAuthentication(options =>
            {
                options.AddScheme<AnfAuthenticationHandler>(AuthenticationConst.SchemeName, "default scheme");
                options.DefaultAuthenticateScheme = AuthenticationConst.SchemeName;
                options.DefaultChallengeScheme = AuthenticationConst.SchemeName;
            });
            services.AddScoped<AnfAuthenticationHandler>();
            services.AddScoped<UserService>();
            services.AddScoped<UserIdentityService>();
            return this;
        }
    }
}
