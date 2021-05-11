using Anf.ChannelModel.KeyGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anf.AzureFunc.Services
{
    public class AnalysisService
    {
        private const string ChaptersKey = "Anf.AzureFunc.Services.AnalysisService.Chapters";
        private const string EntityKey = "Anf.AzureFunc.Services.AnalysisService.Entity";

        private static readonly TimeSpan CacheTime = TimeSpan.FromHours(6);

        private readonly ComicEngine eng;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IDatabase redisDatabase;
        private readonly ILogger logger;

        public AnalysisService(ComicEngine eng, IServiceScopeFactory serviceScopeFactory, IDatabase redisDatabase, ILogger logger)
        {
            this.eng = eng;
            this.serviceScopeFactory = serviceScopeFactory;
            this.redisDatabase = redisDatabase;
            this.logger = logger;
        }

        public async Task<ComicEntity> GetEntityAsync(string url)
        {
            var key = RedisKeyGenerator.Concat(EntityKey, url);
            var val = await redisDatabase.StringGetAsync(key);
            if (val.HasValue)
            {
                var entity = val.Get<ComicEntity>();
                return entity;
            }
            var type = eng.GetComicSourceProviderType(url);
            if (type is null)
            {
                return null;
            }
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ser = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(type.ProviderType);
                var entity = await ser.GetChaptersAsync(url);
                var bytes = JsonSerializer.SerializeToUtf8Bytes(entity);
                await redisDatabase.StringSetAsync(key, bytes, CacheTime);
                return entity;
            }
        }

        public async Task<ComicPage[]> GetChaptersAsync(string url)
        {
            var key = RedisKeyGenerator.Concat(ChaptersKey, url);
            var val = await redisDatabase.StringGetAsync(key);
            if (val.HasValue)
            {
                var pages = val.Get<ComicPage[]>();
                return pages;
            }
            var type = eng.GetComicSourceProviderType(url);
            if (type is null)
            {
                return null;
            }
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ser = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(type.ProviderType);
                try
                {
                    var entity = await ser.GetPagesAsync(url);
                    var bytes = JsonSerializer.SerializeToUtf8Bytes(entity);
                    await redisDatabase.StringSetAsync(key, bytes, CacheTime);
                    return entity;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    throw;
                }
            }
        }

        public async Task<Stream> GetPageAsync(string engUrl,string url)
        {
            var type = eng.GetComicSourceProviderType(engUrl);
            if (type is null)
            {
                return null;
            }
            using (var scope = serviceScopeFactory.CreateScope())
            {
                try
                {
                    var ser = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(type.ProviderType);
                    var entity = await ser.GetImageStreamAsync(url);
                    return entity;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    throw;
                }
            }
        }

    }
}
