using Anf.ChannelModel.Entity;
using Anf.ChannelModel.KeyGenerator;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.WebService
{
    public class ReadingIdentity
    {
        public long BookshelfId { get; set; }

        public string Address { get; set; }
    }
    public class ReadingManager
    {
        private const string ReadingKeysKey = "Anf.WebService.ReadingManager.ReadingKeys";
        private const string ReadingKey = "Anf.WebService.ReadingManager.Reading";

        private readonly IDatabase database;
        private readonly IOptions<ReadingOptions> readingOptions;

        public ReadingManager(IDatabase database, IOptions<ReadingOptions> readingOptions)
        {
            this.database = database;
            this.readingOptions = readingOptions;
        }

        public async Task<AnfBookshelfItem> GetAsync(long bookshelfId, string address)
        {
            var key = RedisKeyGenerator.Concat(ReadingKey, bookshelfId, address);
            var entities = await database.HashGetAllAsync(key);
            return ToBookselfItem(entities);
        }
        public async Task<AnfBookshelfItem[]> BatchGetAsync(ReadingIdentity[] identities)
        {
            var keys = identities.Select(x => RedisKeyGenerator.Concat(ReadingKey, x.BookshelfId, x.Address));
            var batch = database.CreateBatch();
            var tasks = keys.Select(x => batch.HashGetAllAsync(x)).ToArray();
            batch.Execute();
            var entities = await Task.WhenAll(tasks);
            return entities.Where(x => x != null)
                .Select(x => ToBookselfItem(x))
                .ToArray();
        }
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
                UserId = Find<long>(nameof(AnfBookshelfItem.UserId)),
                Address = Find<string>(nameof(AnfBookshelfItem.Address)),
                CreateTime = new DateTime(Find<long>(nameof(AnfBookshelfItem.CreateTime))),
                BookshelfId = Find<long>(nameof(AnfBookshelfItem.BookshelfId)),
                Like = Find<bool>(nameof(AnfBookshelfItem.Like)),
                ReadChatper = Find<int?>(nameof(AnfBookshelfItem.ReadChatper)),
                ReadPage = Find<int?>(nameof(AnfBookshelfItem.ReadPage)),
                UpdateTime = new DateTime(Find<long>(nameof(AnfBookshelfItem.UpdateTime)))
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
                 new HashEntry(nameof(AnfBookshelfItem.UserId),bookshelf.UserId),
            };
        }

        public async Task<int> DoKeysAsync(Func<AnfBookshelfItem[], Task<bool>> func, int pageSize = 250)
        {
            var sc = database.SetScanAsync(ReadingKeysKey, default, pageSize);
            var cur = sc.GetAsyncEnumerator();
            var ps = new List<string>(pageSize);
            var okCount = 0;
            while (await cur.MoveNextAsync())
            {
                ps.Add(cur.Current);
                if (ps.Count >= pageSize)
                {
                    await HandleKeysAsync(ps);
                    okCount += ps.Count;
                    ps.Clear();
                }
            }
            if (ps.Count != 0)
            {
                await HandleKeysAsync(ps);
                okCount += ps.Count;
                ps.Clear();
            }
            return okCount;
            async Task HandleKeysAsync(IEnumerable<string> keys)
            {
                var batch = database.CreateBatch();
                var tasks = keys.Select(x => batch.HashGetAllAsync(x)).ToArray();
                batch.Execute();
                await Task.WhenAll(tasks);
                var entitys = tasks.Where(x => x.Result.Length != 0)
                    .Select(x => ToBookselfItem(x.Result))
                    .ToArray();
                if (entitys.Length != 0)
                {
                    var earse = await func(entitys);
                    if (earse)
                    {
                        var values = keys.Select(x => new RedisValue(x)).ToArray();
                        await database.SetRemoveAsync(ReadingKeysKey, values);
                    }
                }
            }
        }
        public async Task RemoveAsync(long bookshelfId, string address)
        {
            var key = RedisKeyGenerator.Concat(ReadingKey, bookshelfId, address);
            await database.SetRemoveAsync(ReadingKeysKey,key);
            await database.KeyDeleteAsync(key);
        }
        public async Task SetAsync(long userId, long bookshelfId, string address, int? chapter, int? page,bool? like)
        {
            var key = RedisKeyGenerator.Concat(ReadingKey, bookshelfId, address);
            var exists = await database.KeyExistsAsync(key);
            var now = DateTime.Now;
            if (exists)
            {
                var count = 0;
                if (chapter != null)
                {
                    count++;
                }
                if (page != null)
                {
                    page++;
                }
                if (like!=null)
                {
                    page++;
                }
                if (count == 0)
                {
                    return;
                }
                var entity = new List<HashEntry>(count);
                if (chapter != null)
                {
                    entity.Add(new HashEntry(nameof(AnfBookshelfItem.ReadChatper), chapter.Value));
                }
                if (page != null)
                {
                    entity.Add(new HashEntry(nameof(AnfBookshelfItem.ReadPage), page.Value));
                }
                if (like!=null)
                {
                    entity.Add(new HashEntry(nameof(AnfBookshelfItem.Like), like.Value));
                }
                await database.HashSetAsync(key, entity.ToArray());
            }
            else
            {
                var model = new AnfBookshelfItem
                {
                    Address = address,
                    ReadChatper = chapter ?? 0,
                    ReadPage = page ?? 0,
                    CreateTime = now,
                    UpdateTime = now,
                    BookshelfId = bookshelfId,
                    Like = like ?? false,
                    UserId= userId
                };
                var hashs = AsEntities(model);
                await database.HashSetAsync(key, hashs);
            }
            await database.KeyExpireAsync(key, readingOptions.Value.ReadingTimeout);
            await database.SetAddAsync(ReadingKeysKey, key);
        }
    }
    /*
     * set->(阅读键s)
     * 使用时增加阅读键，任何定时同步
     */
}
