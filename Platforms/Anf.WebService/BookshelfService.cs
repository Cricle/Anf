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
        private static readonly string BookshelfItemCreateKey = "Anf.ResourceFetcher.Services.BookshelfService.BookshelfItemCreate";
        private static readonly string RedBookshelfItemOpKey = "Red.Anf.ResourceFetcher.Services.BookshelfService.RedBookshelfItemOp";
        private static readonly string RedBookshelfOpKey = "Red.Anf.ResourceFetcher.Services.BookshelfService.BookshelfOp";

        private readonly IDatabase redisDatabase;
        private readonly IDistributedLockFactory distributedLockFactory;
        private readonly AnfDbContext dbContext;
        private readonly ReadingManager readingManager;
        private readonly IOptions<BookshelfOptions> bookshelfOptions;

        public BookshelfService(IDatabase redisDatabase, IDistributedLockFactory distributedLockFactory, AnfDbContext dbContext, ReadingManager readingManager, IOptions<BookshelfOptions> bookshelfOptions)
        {
            this.redisDatabase = redisDatabase;
            this.distributedLockFactory = distributedLockFactory;
            this.dbContext = dbContext;
            this.readingManager = readingManager;
            this.bookshelfOptions = bookshelfOptions;
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
                Id = Find<long>(nameof(AnfBookshelf.Id)),
                LinkId = Find<long?>(nameof(AnfBookshelf.LinkId)),
                Name = Find<string>(nameof(AnfBookshelf.Name)),
                UserId = Find<long>(nameof(AnfBookshelf.UserId)),
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

        private async Task<AnfBookshelf> GetBookshelfFromCahceAsync(long id)
        {
            var key = RedisKeyGenerator.Concat(BookshelfKey, id);
            var entities = await redisDatabase.HashGetAllAsync(key);
            return ToBookself(entities);
        }
        public async Task<bool> DeleteBookshelfAsync(long userId, long id)
        {
            var lockKey = RedisKeyGenerator.Concat(RedBookshelfOpKey, id);
            using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedisKeyGenerator.RedKeyOutTime))
            {
                if (locker.IsAcquired)
                {
                    return false;
                }
                await dbContext.Bookshelves.AsNoTracking()
                    .Where(x => x.Id == id&&x.UserId==userId)
                    .Take(1)
                    .DeleteFromQueryAsync();
                var bkkey = RedisKeyGenerator.Concat(BookshelfKey, id);
                await redisDatabase.KeyDeleteAsync(bkkey);
                return true;
            }
        }
        public async Task<bool> DeleteBookshelfItemAsync(long bookshelfId, string address)
        {
            var lockKey = RedisKeyGenerator.Concat(RedBookshelfItemOpKey, bookshelfId, address);
            using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedisKeyGenerator.RedKeyOutTime))
            {
                if (locker.IsAcquired)
                {
                    return false;
                }
                await dbContext.BookshelfItems.AsNoTracking()
                    .Where(x => x.BookshelfId == bookshelfId && x.Address == address)
                    .Take(1)
                    .DeleteFromQueryAsync();
                var createKey = RedisKeyGenerator.Concat(BookshelfItemCreateKey, bookshelfId, address);
                await redisDatabase.KeyDeleteAsync(createKey);
                await readingManager.RemoveAsync(bookshelfId, address);
                return true;
            }
        }
        public async Task CreateBookshelfAsync(long userId, string name)
        {
            var entity = new AnfBookshelf
            {
                UserId = userId,
                Name = name,
                CreateTime = DateTime.Now
            };
            await dbContext.Bookshelves.SingleInsertAsync(entity);
        }
        public Task StoreAsync(int pageSize = 250)
        {
            return readingManager.DoKeysAsync(CoreStoreAsync, pageSize);
        }
        private async Task<bool> CoreStoreAsync(AnfBookshelfItem[] items)
        {
            var updateColumns = new List<string>
            {
                nameof(AnfBookshelfItem.ReadChatper),
                nameof(AnfBookshelfItem.ReadPage),
                nameof(AnfBookshelfItem.Like),
            };
            await dbContext.BookshelfItems.BulkUpdateAsync(items, opt =>
            {
                opt.UpdateMatchedAndConditionNames = updateColumns;
                opt.ColumnPrimaryKeyExpression = x => new { x.BookshelfId, x.Address };
            });
            return true;
        }
        public async Task<bool> UpdateBookshelfAsync(long id, string name, bool like)
        {
            var lockKey = RedisKeyGenerator.Concat(RedLoadBookshelfKey, id);
            using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedisKeyGenerator.RedKeyOutTime))
            {
                if (!locker.IsAcquired)
                {
                    return false;
                }
                var count = await dbContext.Bookshelves.AsNoTracking()
                    .Where(x => x.Id == id)
                    .UpdateFromQueryAsync(x => new AnfBookshelf
                    {
                        Name = name,
                        Like = like
                    });
                if (count != 0)
                {
                    var bkkey = RedisKeyGenerator.Concat(BookshelfKey, id);
                    await redisDatabase.HashSetAsync(bkkey, nameof(AnfBookshelf.Name), name, When.Exists);
                    await redisDatabase.HashSetAsync(bkkey, nameof(AnfBookshelf.Like), like, When.Exists);
                }
                return count != 0;
            }
        }
        public async Task<bool> UpdateBookshelfItemAsync(long userId, long bookshelfId, string address, int? chapter, int? page, bool? like)
        {
            var createKey = RedisKeyGenerator.Concat(BookshelfItemCreateKey, bookshelfId, address);
            var exists = await redisDatabase.KeyExistsAsync(createKey);
            if (!exists)
            {
                var lockKey = RedisKeyGenerator.Concat(RedBookshelfItemOpKey, bookshelfId, address);
                using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedisKeyGenerator.RedKeyOutTime))
                {
                    if (locker.IsAcquired)
                    {
                        return false;
                    }
                    exists = await redisDatabase.KeyExistsAsync(createKey);
                    if (!exists)
                    {
                        exists = await dbContext.BookshelfItems.AsNoTracking()
                            .Where(x => x.BookshelfId == bookshelfId && x.Address == address)
                            .AnyAsync();
                        if (!exists)
                        {
                            var now = DateTime.Now;
                            var item = new AnfBookshelfItem
                            {
                                BookshelfId = bookshelfId,
                                Address = address,
                                CreateTime = now,
                                ReadPage = page,
                                ReadChatper = chapter,
                                Like = like ?? false,
                                UpdateTime = now,
                                UserId = userId
                            };
                            await dbContext.BookshelfItems.SingleInsertAsync(item);
                        }
                        await redisDatabase.StringSetAsync(createKey, true, bookshelfOptions.Value.BookshelfCreateCacheTimeout);
                    }
                }
            }
            await readingManager.SetAsync(userId,bookshelfId, address, chapter, page, like);
            return true;
        }
        public async Task<AnfBookshelfItem> GetBookshelfItemAsync(long userId, long bookshelfId, string address)
        {
            var res = await readingManager.GetAsync(bookshelfId, address);
            if (res is null)
            {
                var lockKey = RedisKeyGenerator.Concat(RedBookshelfItemOpKey, bookshelfId, address);
                using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedisKeyGenerator.RedKeyOutTime))
                {
                    if (!locker.IsAcquired)
                    {
                        return null;
                    }
                    res = await readingManager.GetAsync(bookshelfId, address);
                    if (res != null)
                    {
                        return res;
                    }
                    res = await dbContext.BookshelfItems.AsNoTracking()
                        .Where(x => x.BookshelfId == bookshelfId && x.Address == address)
                        .FirstOrDefaultAsync();
                    if (res != null)
                    {
                        await readingManager.SetAsync(userId,res.BookshelfId, res.Address, res.ReadChatper, res.ReadPage, res.Like);
                    }
                }
            }
            return res;
        }
        public async Task<AnfBookshelf> GetBookshelfAsync(long id)
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
