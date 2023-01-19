using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace Anf.Web
{
    internal partial class WebModuleEntry
    {
        public WebModuleEntry AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("Anf", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Anf API"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                foreach (var item in Program.modules)
                {
                    var mFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    if (File.Exists(mFile))
                    {
                        var mPath = Path.Combine(AppContext.BaseDirectory, mFile);
                        c.IncludeXmlComments(xmlPath);
                    }

                }
            });
            return this;
        }
    }
}
