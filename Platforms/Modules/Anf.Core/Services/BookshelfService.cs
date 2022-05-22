using Anf.ChannelModel.Entity;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RedLockNet;
using SecurityLogin;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.WebService
{
    public class BookshelfService
    {
        private static readonly TimeSpan RedKeyOutTime = TimeSpan.FromSeconds(5);

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
                 new HashEntry(nameof(AnfBookshelf.Name),bookshelf.Name??RedisValue.EmptyString),
                 new HashEntry(nameof(AnfBookshelf.UserId),bookshelf.UserId),
                 new HashEntry(nameof(AnfBookshelf.Like),bookshelf.Like),
                 new HashEntry(nameof(AnfBookshelf.LinkId),bookshelf.LinkId??RedisValue.EmptyString),
                 new HashEntry(nameof(AnfBookshelf.Id),bookshelf.Id),
            };
        }

        private async Task<AnfBookshelf> GetBookshelfFromCahceAsync(long id)
        {
            var key = KeyGenerator.Concat(BookshelfKey, id);
            var entities = await redisDatabase.HashGetAllAsync(key);
            return ToBookself(entities);
        }
        public async Task<bool> DeleteBookshelfAsync(long userId, long id)
        {
            var lockKey = KeyGenerator.Concat(RedBookshelfOpKey, id);
            using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedKeyOutTime))
            {
                if (locker.IsAcquired)
                {
                    return false;
                }
                await dbContext.Bookshelves.AsNoTracking()
                    .Where(x => x.Id == id && x.UserId == userId)
                    .Take(1)
                    .BatchDeleteAsync();
                var bkkey = KeyGenerator.Concat(BookshelfKey, id);
                await redisDatabase.KeyDeleteAsync(bkkey);
                return true;
            }
        }
        public async Task<bool> DeleteBookshelfItemAsync(long bookshelfId, string address)
        {
            var lockKey = KeyGenerator.Concat(RedBookshelfItemOpKey, bookshelfId, address);
            using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedKeyOutTime))
            {
                if (locker.IsAcquired)
                {
                    return false;
                }
                await dbContext.BookshelfItems.AsNoTracking()
                    .Where(x => x.BookshelfId == bookshelfId && x.Address == address)
                    .Take(1)
                    .BatchDeleteAsync();
                var createKey = KeyGenerator.Concat(BookshelfItemCreateKey, bookshelfId, address);
                await redisDatabase.KeyDeleteAsync(createKey);
                await readingManager.RemoveAsync(bookshelfId, address);
                return true;
            }
        }
        public async Task CreateBookshelfAsync(long userId, string name)
        {
            var entity = new AnfBookshelf
            {
                Id = GuidToLong(),
                UserId = userId,
                Name = name,
                CreateTime = DateTime.Now
            };
            dbContext.Bookshelves.Add(entity);
            await dbContext.SaveChangesAsync();
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
            await dbContext.BookshelfItems.BatchUpdateAsync(items, updateColumns);
            return true;
        }
        public async Task<bool> UpdateBookshelfAsync(long id, string name, bool like)
        {
            var lockKey = KeyGenerator.Concat(RedLoadBookshelfKey, id);
            using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedKeyOutTime))
            {
                if (!locker.IsAcquired)
                {
                    return false;
                }
                var count = await dbContext.Bookshelves.AsNoTracking()
                    .Where(x => x.Id == id)
                    .Take(1)
                    .BatchUpdateAsync(x => new AnfBookshelf
                    {
                        Name = name,
                        Like = like
                    });
                if (count != 0)
                {
                    var bkkey = KeyGenerator.Concat(BookshelfKey, id);
                    await redisDatabase.HashSetAsync(bkkey, nameof(AnfBookshelf.Name), name, When.Exists);
                    await redisDatabase.HashSetAsync(bkkey, nameof(AnfBookshelf.Like), like, When.Exists);
                }
                return count != 0;
            }
        }
        public async Task<bool> UpdateBookshelfItemAsync(long userId, long bookshelfId, string address, int? chapter, int? page, bool? like)
        {
            var createKey = KeyGenerator.Concat(BookshelfItemCreateKey, bookshelfId, address);
            var exists = await redisDatabase.KeyExistsAsync(createKey);
            if (!exists)
            {
                var lockKey = KeyGenerator.Concat(RedBookshelfItemOpKey, bookshelfId, address);
                using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedKeyOutTime))
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
                            dbContext.BookshelfItems.Add(item);
                            await dbContext.SaveChangesAsync();
                        }
                        await redisDatabase.StringSetAsync(createKey, true, bookshelfOptions.Value.BookshelfCreateCacheTimeout);
                    }
                }
            }
            await readingManager.SetAsync(userId, bookshelfId, address, chapter, page, like);
            return true;
        }
        public async Task<AnfBookshelfItem> GetBookshelfItemAsync(long userId, long bookshelfId, string address)
        {
            var res = await readingManager.GetAsync(bookshelfId, address);
            if (res is null)
            {
                var lockKey = KeyGenerator.Concat(RedBookshelfItemOpKey, bookshelfId, address);
                using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedKeyOutTime))
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
                        await readingManager.SetAsync(userId, res.BookshelfId, res.Address, res.ReadChatper, res.ReadPage, res.Like);
                    }
                }
            }
            return res;
        }
        public Task<AnfBookshelf> GetBookshelfAndItemsAsync(long id)
        {
            return dbContext.Bookshelves.AsNoTracking()
                .Where(x => x.Id == id)
                .Include(x => x.Items)
                .FirstOrDefaultAsync();
        }
        private static long GuidToLong()
        {
            var uid = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(uid, 0);
        }
        public async Task<AnfBookshelf> GetBookshelfAsync(long id)
        {
            var res = await GetBookshelfFromCahceAsync(id);
            if (res is null)
            {
                var lockKey = KeyGenerator.Concat(RedLoadBookshelfKey, id);
                using (var locker = await distributedLockFactory.CreateLockAsync(lockKey, RedKeyOutTime))
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
                        var bkkey = KeyGenerator.Concat(BookshelfKey, id);
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
