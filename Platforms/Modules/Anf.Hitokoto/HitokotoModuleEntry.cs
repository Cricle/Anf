using Anf.ChannelModel;
using Anf.ChannelModel.Entity;
using Anf.Hitokoto.Caching;
using Anf.WebService;
using Ao.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Structing;
using Structing.Core;
using Structing.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EFCore.BulkExtensions;

namespace Anf.Hitokoto
{
    [EnableApplicationPart]
    public class HitokotoModuleEntry : AutoModuleEntry
    {
        public override void Register(IRegisteContext context)
        {
            base.Register(context);

            var services = context.Services;

            services.AddScoped<IDataAccesstor<ulong, WordResponse>, WordDataAccessor>();
            services.AddSingleton<RandomWordResultCacheFinder>();
        }

        public override async Task AfterReadyAsync(IReadyContext context)
        {
            await CreateSeedDataAsync(context);
            await base.AfterReadyAsync(context);
        }

        private async Task CreateSeedDataAsync(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AnfDbContext>();
                db.Database.EnsureCreated();
                if (!db.Words.Any())
                {
                    var um = scope.ServiceProvider.GetRequiredService<UserManager<AnfUser>>();
                    await um.CreateAsync(new AnfUser
                    {
                        NormalizedUserName = "test",
                        UserName = "test",
                    }, "test123");
                    var u = await um.FindByNameAsync("test");
                    var word = new List<AnfWord>();
                    foreach (var item in Directory.EnumerateFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sentences"), "*.json"))
                    {
                        using (var fs = File.Open(item, FileMode.Open))
                        {
                            var obj = JsonDocument.Parse(fs);
                            using (var enu = obj.RootElement.EnumerateArray())
                            {
                                while (enu.MoveNext())
                                {
                                    var t = DateTime.Now;
                                    var ts = enu.Current.GetProperty("created_at").GetString();
                                    if (!string.IsNullOrEmpty(ts))
                                    {
                                        var lts = long.Parse(ts);
                                        if (ts.Length != 13)
                                        {
                                            lts *= 1000;
                                        }
                                        t = TimeHelper.GetCsTime(lts);
                                    }
                                    var w = new AnfWord
                                    {
                                        Text = enu.Current.GetProperty("hitokoto").ToString(),
                                        From = enu.Current.GetProperty("from").ToString(),
                                        Type = (WordType)enu.Current.GetProperty("type").GetString()[0],
                                        CommitType = CommitTypes.Web,
                                        CreatorId = u.Id,
                                        Length = (ushort)enu.Current.GetProperty("length").GetInt32(),
                                        CreateTime = t,
                                    };
                                    word.Add(w);
                                }
                            }
                        }
                    }
                    await db.AddRangeAsync(word);

                    db.Apps.Add(new AnfApp
                    {
                        AppKey = "59d748bbd3ab4d30bb2991fee61b09db",
                        AppSecret = "3FB30BC571AC886FD1400BDAC95B84A1",
                        Enable = true,
                        CreateTime = DateTime.Parse("2022-03-29 17:13:30.3424143"),
                        UserId = 1
                    });
                    db.SaveChanges();
                }
            }
        }
    }
}
