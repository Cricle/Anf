using Anf.ChannelModel.Entity;
using Anf.ChannelModel.KeyGenerator;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RedLockNet.SERedis;
using RedLockNet;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Anf.WebService
{
    public class BookshelfService
    {
        private static readonly string RedLoadBookshelfKey = "Red.Anf.ResourceFetcher.Services.BookshelfService.LoadBookshelf";
        private static readonly string BookshelfKey = "Anf.ResourceFetcher.Services.BookshelfService.Bookshelf";
        private static readonly string BookshelfItemKey = "Anf.ResourceFetcher.Services.BookshelfService.BookshelfItem";

        private readonly IDatabase redisDatabase;
        private readonly IDistributedLockFactory distributedLockFactory;
        private readonly AnfDbContext dbContext;
        private readonly IOptions<BookshelfOptions> bookshelfOptions;

        private AnfBookshelfItem ToBookselfItem(HashEntry[] entries)
        {
            if (entries.Length == 0)
            {
                return null;
            }
            var map = entries.ToDictionary();
            T Find<T>(string name)
            {
                if (map.TryGetValue(name, out var val))
                {
                    return val.Get<T>();
                }
                return default;
            }
            return new AnfBookshelfItem
            {
                Address = Find<string>(nameof(AnfBookshelfItem.Address)),
                CreateTime = new DateTime(Find<long>(nameof(AnfBookshelfItem.CreateTime))),
                BookshelfId = Find<ulong>(nameof(AnfBookshelfItem.BookshelfId)),
                Like = Find<bool>(nameof(AnfBookshelfItem.Like)),
                ReadChatper = Find<int?>(nameof(AnfBookshelfItem.ReadChatper)),
                ReadPage = Find<int?>(nameof(AnfBookshelfItem.ReadPage)),
                UpdateTime = new DateTime(Find<long>(nameof(AnfBookshelfItem.UpdateTime)))
            };
        }
        private AnfBookshelf ToBookself(HashEntry[] entries)
        {
            if (entries.Length == 0)
            {
                return null;
            }
            var map = entries.ToDictionary();
            T Find<T>(string name)
            {
                if (map.TryGetValue(name, out var val))
                {
                    return val.Get<T>();
                }
                return default;
            }
            return new AnfBookshelf
            {
                CreateTime = new DateTime(Find<long>(nameof(AnfBookshelf.CreateTime))),
                Like = Find<bool>(nameof(AnfBookshelf.Like)),
                Id = Find<ulong>(nameof(AnfBookshelf.Id)),
                LinkId = Find<ulong?>(nameof(AnfBookshelf.LinkId)),
                Name = Find<string>(nameof(AnfBookshelf.Name)),
                UserId = Find<long>(nameof(AnfBookshelf.UserId)),
            };
        }
        private HashEntry[] AsEntities(AnfBookshelfItem bookshelf)
        {
            return new HashEntry[]
            {
                 new HashEntry(nameof(AnfBookshelfItem.CreateTime),bookshelf.CreateTime.Ticks),
                 new HashEntry(nameof(AnfBookshelfItem.BookshelfId),bookshelf.BookshelfId),
                 new HashEntry(nameof(AnfBookshelfItem.Address),bookshelf.Address),
                 new HashEntry(nameof(AnfBookshelfItem.ReadChatper),bookshelf.ReadChatper??RedisValue.EmptyString),
                 new HashEntry(nameof(AnfBookshelfItem.ReadPage),bookshelf.ReadPage??RedisValue.EmptyString),
                 new HashEntry(nameof(AnfBookshelfItem.UpdateTime),bookshelf.UpdateTime?.Ticks??0),
            };
        }
        private HashEntry[] AsEntities(AnfBookshelf bookshelf)
        {
            return new HashEntry[]
            {
                 new HashEntry(nameof(AnfBookshelf.CreateTime),bookshelf.CreateTime.Ticks),
                 new HashEntry(nameof(AnfBookshelf.Name),bookshelf.Name),
                 new HashEntry(nameof(AnfBookshelf.UserId),bookshelf.UserId),
                 new HashEntry(nameof(AnfBookshelf.Like),bookshelf.Like),
                 new HashEntry(nameof(AnfBookshelf.LinkId),bookshelf.LinkId),
                 new HashEntry(nameof(AnfBookshelf.Id),bookshelf.Id),
            };
        }

        private async Task<AnfBookshelf> GetBookshelfFromCahceAsync(ulong id)
        {
            var key = RedisKeyGenerator.Concat(BookshelfKey, id);
            var entities = await redisDatabase.HashGetAllAsync(key);
            return ToBookself(entities);
        }
        private async Task<AnfBookshelfItem> GetBookshelfItemsFromCahceAsync(ulong id)
        {
            var key = RedisKeyGenerator.Concat(BookshelfItemKey, id);
            var entities = await redisDatabase.HashGetAllAsync(key);
            return ToBookselfItem(entities);
        }
        public async Task<AnfBookshelfItem> GetBookshelfItemAsync(ulong id, string address)
        {
            var res = await GetBookshelfItemsFromCahceAsync(id);
            if (res is null)
            {
                var lockKey = RedisKeyGenerator.Concat(RedLoadBookshelfKey, id);
                using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedisKeyGenerator.RedKeyOutTime))
                {
                    if (!locker.IsAcquired)
                    {
                        return null;
                    }
                    res = await GetBookshelfItemsFromCahceAsync(id);
                    if (res != null)
                    {
                        return res;
                    }
                    res = await dbContext.BookshelfItems.AsNoTracking()
                        .Where(x => x.BookshelfId == id && x.Address == address)
                        .FirstOrDefaultAsync();
                    if (res != null)
                    {
                        var bkkey = RedisKeyGenerator.Concat(BookshelfItemKey, id);
                        var hashs = AsEntities(res);
                        await redisDatabase.HashSetAsync(bkkey, hashs);
                        await redisDatabase.KeyExpireAsync(bkkey, bookshelfOptions.Value.BookshelfItemTimeout);
                    }
                }
            }
            return res;
        }
        public async Task<AnfBookshelf> GetBookshelfAsync(ulong id)
        {
            var res = await GetBookshelfFromCahceAsync(id);
            if (res is null)
            {
                var lockKey = RedisKeyGenerator.Concat(RedLoadBookshelfKey, id);
                using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedisKeyGenerator.RedKeyOutTime))
                {
                    if (!locker.IsAcquired)
                    {
                        return null;
                    }
                    res = await GetBookshelfFromCahceAsync(id);
                    if (res != null)
                    {
                        return res;
                    }
                    res = await dbContext.Bookshelves.AsNoTracking()
                        .Include(x=>x.Items)
                        .Where(x => x.Id == id)
                        .FirstOrDefaultAsync();
                    if (res != null)
                    {
                        var bkkey = RedisKeyGenerator.Concat(BookshelfKey, id);
                        var hashs = AsEntities(res);
                        await redisDatabase.HashSetAsync(bkkey, hashs);
                        await redisDatabase.KeyExpireAsync(bkkey, bookshelfOptions.Value.BookshelfTimeout);
                    }
                }
            }
            return res;
        }
    }
}
