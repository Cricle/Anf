using Anf.Easy.Store;
using Ao.Cache.MessagePack.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Core.Finders
{
    public class ComicImageIdentity:IEquatable<ComicImageIdentity>
    {
        public ComicImageIdentity(string entityUrl, string url)
        {
            EntityUrl = entityUrl ?? throw new ArgumentNullException(nameof(entityUrl));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public string EntityUrl { get; }

        public string Url { get; }

        public override string ToString()
        {
            return $"{EntityUrl}_{Url}";
        }
        public bool Equals(ComicImageIdentity other)
        {
            if (other ==null)
            {
                return false;
            }
            return other.EntityUrl==this.EntityUrl && other.Url==this.Url;
        }
        public override int GetHashCode()
        {
            return EntityUrl.GetHashCode() ^ Url.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as ComicImageIdentity);
        }
    }
    public class ComicImageFinder : RedisMessagePackDataFinder<ComicImageIdentity, string>
    {
        public ComicImageFinder(IDatabase database, 
            IServiceScopeFactory scopeFactory,
            ComicEngine comicEngine,
            ILoggerFactory loggerFactory)
        {
            Database = database;
            this.comicEngine = comicEngine;
            this.scopeFactory = scopeFactory;
            this.loggerFactory = loggerFactory;
        }

        public IDatabase Database { get; }

        private readonly IServiceScopeFactory scopeFactory;
        private readonly ComicEngine comicEngine;
        private readonly ILoggerFactory loggerFactory;

        protected override IDatabase GetDatabase()
        {
            return Database;
        }
        protected override async Task<string> OnFindInDbAsync(ComicImageIdentity identity)
        {
            using var scope = scopeFactory.CreateScope();
            var prov = comicEngine.GetComicSourceProviderType(identity.EntityUrl);
            if (prov is null)
            {
                return string.Empty;
            }
            var provider = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(prov.ProviderType);
            var storeSer = scope.ServiceProvider.GetRequiredService<IStoreService>();
            var imgRes = await storeSer.GetPathAsync(identity.Url);
            var exists = await storeSer.ExistsAsync(identity.Url);
            if (!exists)
            {
                try
                {
                    using (var img = await provider.GetImageStreamAsync(identity.Url))
                    {
                        await storeSer.SaveAsync(identity.Url, img);
                    }
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<ComicImageFinder>();
                    logger.LogError(ex,identity.Url);
                }
            }
            return imgRes;
        }
        protected override TimeSpan? GetCacheTime(ComicImageIdentity identity, string entity)
        {
            return TimeSpan.FromMinutes(3);
        }
    }
}
