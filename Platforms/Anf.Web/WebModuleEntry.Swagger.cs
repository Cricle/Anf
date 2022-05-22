using Anf.WebService;
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
                c.AddSecurityDefinition(AuthenticationConst.APPKeyHeader, new OpenApiSecurityScheme
                {
                    Name = AuthenticationConst.APPKeyHeader,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = AuthenticationConst.APPKeyHeader
                });
                c.AddSecurityDefinition(AuthenticationConst.AccessHeader, new OpenApiSecurityScheme
                {
                    Name = AuthenticationConst.AccessHeader,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = AuthenticationConst.AccessHeader
                });
                c.AddSecurityDefinition(AuthenticationConst.AuthHeader, new OpenApiSecurityScheme
                {
                    Name = AuthenticationConst.AuthHeader,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = AuthenticationConst.AuthHeader
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type= ReferenceType.SecurityScheme,
                                Id=AuthenticationConst.AccessHeader
                            },
                            Scheme= AuthenticationConst.AccessHeader,
                            In= ParameterLocation.Header
                        },Array.Empty<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type= ReferenceType.SecurityScheme,
                                Id=AuthenticationConst.APPKeyHeader
                            },
                            Scheme= AuthenticationConst.APPKeyHeader,
                            In= ParameterLocation.Header
                        },Array.Empty<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type= ReferenceType.SecurityScheme,
                                Id=AuthenticationConst.AuthHeader
                            },
                            Scheme= AuthenticationConst.AuthHeader,
                            In= ParameterLocation.Header
                        },Array.Empty<string>()
                    }
                });
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
