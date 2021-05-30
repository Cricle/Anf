using Anf.Easy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Anf.KnowEngines;
using System.IO;
using Anf.Easy.Store;
using System;
using Anf.Easy.Visiting;
using Anf.Engine;
using Anf.Web.Hubs;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Caching.Distributed;
using Anf.ResourceFetcher;
using Anf.ChannelModel.Entity;
using Anf.ResourceFetcher.Fetchers;
using Anf.WebService;
using Anf.ResourceFetcher.Redis;
using Quartz.Impl;
using System.Threading.Tasks;
using Quartz;
using Anf.Web.Jobs;
using System.Linq;

namespace Anf.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AppEngine.Reset(services);
            AppEngine.AddServices(NetworkAdapterTypes.HttpClient);
            var store = FileStoreService.FromMd5Default(Path.Combine(Environment.CurrentDirectory, XComicConst.CacheFolderName));

            services.AddSingleton<IComicSaver>(store);
            services.AddSingleton<IStoreService>(store);
            services.AddSingleton<IResourceFactoryCreator<Stream>, WebResourceFactoryCreator>();
            services.AddSingleton<ProposalEngine>();
            services.AddScoped<IComicVisiting<Stream>, WebComicVisiting>();
            services.AddKnowEngines();
            services.AddControllersWithViews();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTSCONNECTIONSTRING"]);
            services.AddSignalR()
                .AddAzureSignalR();
            services.AddDbContext<AnfDbContext>((x, y) =>
            {
                var config = x.GetRequiredService<IConfiguration>();
                y.UseSqlServer(config["ConnectionStrings:anfdb"]);
            }, optionsLifetime: ServiceLifetime.Singleton)
            .AddDbContextPool<AnfDbContext>((x,y)=> 
            {
                var config = x.GetRequiredService<IConfiguration>();
                y.UseSqlServer(config["ConnectionStrings:anfdb"]);
            }).AddIdentity<AnfUser,AnfRole>(x=> 
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AnfDbContext>();
            services.AddSingleton<IDistributedCache, RedisCache>();
            services.AddOptions<RedisCacheOptions>()
                .Configure(x => x.Configuration = Configuration["ConnectionStrings:CacheConnection"]);
            services.AddResponseCompression();
            services.AddScoped<UserService>();
            services.AddScoped<UserIdentityService>();
            services.AddSwaggerGen(c=> 
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
            });
            services.AddMemoryCache();
            services.AddAuthorization();
            services.AddAuthentication(options => 
            {
                options.AddScheme<AnfAuthenticationHandler>(AnfAuthenticationHandler.SchemeName, "default scheme");
                options.DefaultAuthenticateScheme = AnfAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = AnfAuthenticationHandler.SchemeName;
            });

            services.AddScoped<AnfAuthenticationHandler>();
            services.AddScoped<ComicRankService>();
            services.AddScoped<ComicRankSaver>();
            services.AddScoped<BookshelfService>();
            services.AddScoped<HotSearchService>();

            services.AddOptions<BookshelfOptions>();
            services.AddOptions<ComicRankOptions>();
            services.AddOptions<FetchOptions>();
            services.AddOptions<ResourceLockOptions>();
            services.AddFetcherProvider()
                .AddRedisFetcherProvider()
                .AddMssqlResourceFetcher()
                .AddDefaultFetcherProvider();
            services.AddResourceFetcher()
                .AddMssqlResourceFetcher()
                .AddRedisFetcherProvider();
            services.AddScoped<IDbContextTransfer, AnfDbContextTransfer>();
            services.AddRedis();
            services.AddScoped<ReadingManager>();
            AddQuartzAsync(services).GetAwaiter().GetResult();
        }
        private async Task AddQuartzAsync(IServiceCollection services)
        {
            services.AddScoped<StoreBookshelfJob>();
            services.AddScoped<SaveRankJob>();

            var factory = new StdSchedulerFactory();
            var schedule =await factory.GetScheduler();
            await schedule.Start();
            services.AddSingleton<ISchedulerFactory>(factory);
            services.AddSingleton<ISingletonJobFactory>(x=> 
            {
                var fc = x.GetRequiredService<IServiceScopeFactory>();
                var w = x.GetRequiredService<ISchedulerFactory>();
                return new SingletonJobFactory(fc,w);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseResponseCompression();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAzureSignalR(builder =>
            {
                builder.MapHub<ReadingHub>("/hubs/v1/reading");
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/Anf/swagger.json", "Anf API");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                    //spa.UseAngularCliServer(npmScript: "start");
                }
            });

            app.ApplicationServices.UseKnowEngines();
            var eng = app.ApplicationServices.GetComicEngine();
            var tx = eng.FirstOrDefault(x => x is TencentComicSourceCondition);
            if (tx!=null)
            {
                eng.Remove(tx);
            }
            //using (var s = app.ApplicationServices.GetServiceScope())
            //{
            //    var db = s.ServiceProvider.GetRequiredService<AnfDbContext>();
            //    db.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
            //    db.Database.EnsureCreated();
            //}
            var scope = app.ApplicationServices.CreateScope();
            //_ = AnfMongoDbExtensions.InitMongoAsync(scope);
            InitJobAsync(scope).GetAwaiter().GetResult();
        }
        private async Task InitJobAsync(IServiceScope scope)
        {
            using (scope)
            {
                var f = scope.ServiceProvider.GetRequiredService<ISingletonJobFactory>();

                var now = DateTime.Now;
                async Task ScheduleSaveRankeAsync(RankLevels level)
                {
                    var jobIdentity = JobBuilder.Create<SaveRankJob>()
                        .WithIdentity(nameof(SaveRankJob)+ level.ToString())
                        .RequestRecovery()
                        .Build();
                    TimeSpan rep = default;
                    DateTime startTime = default;
                    if (level == RankLevels.Hour)
                    {
                        rep = TimeSpan.FromHours(1);
                        startTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 1, 0).AddHours(1);
                    }
                    else if (level == RankLevels.Day)
                    {
                        rep = TimeSpan.FromDays(1);
                        startTime = new DateTime(now.Year, now.Month, now.Day, 0, 30, 0).AddDays(1);;
                    }
                    else if (level == RankLevels.Month)
                    {
                        rep = TimeSpan.FromDays(32);
                        startTime = new DateTime(now.Year, now.Month, 1, 1, 0, 0).AddMonths(1);
                    }

                    var trigger = TriggerBuilder.Create()
                        .StartAt(new DateTimeOffset(startTime))
                        .WithSimpleSchedule(b =>
                        {
                            b.WithInterval(rep).RepeatForever();
                        })
                        .Build();
                    var scheduler = await f.GetSchedulerAsync();
                    var offset = await scheduler.ScheduleJob(jobIdentity, trigger);
                }

                await ScheduleSaveRankeAsync(RankLevels.Hour);
                await ScheduleSaveRankeAsync(RankLevels.Day);
                await ScheduleSaveRankeAsync(RankLevels.Month);

                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var syncInterval = config.GetValue<int>("BookshelfSync:IntervalS");

                if (syncInterval<=0)
                {
                    syncInterval = 60 * 5;
                }

                var jobIdentity = JobBuilder.Create<StoreBookshelfJob>()
                        .WithIdentity(nameof(StoreBookshelfJob))
                        .RequestRecovery()
                        .Build();
                var trigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithSimpleSchedule(b =>
                    {
                        b.WithIntervalInSeconds(syncInterval).RepeatForever();
                    })
                    .Build();
                var scheduler = await f.GetSchedulerAsync();
                var offset = await scheduler.ScheduleJob(jobIdentity, trigger);
            }
        }
    }
}
