﻿using Anf.ChannelModel.Entity;
using Anf.ChannelModel.Mongo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public class MssqlFetcher : IResourceFetcher
    {
        private readonly IOptions<FetchOptions> fetchOptions;
        private readonly AnfDbContext anfDbContext;

        public MssqlFetcher(IOptions<FetchOptions> fetchOptions, AnfDbContext anfDbContext)
        {
            this.fetchOptions = fetchOptions;
            this.anfDbContext = anfDbContext;
        }

        public async Task DoneFetchChapterAsync(IValueResourceMonitorContext<WithPageChapter> context)
        {
            if (context.ProviderFetcher != this)
            {
                var exists = await anfDbContext.ComicChapters.AsNoTracking()
                    .Where(x => x.TargetUrl == context.Url)
                    .AnyAsync();
                var now = DateTime.Now.Ticks;
                if (exists)
                {
                    var pageJson = Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(context.Value.Pages));
                    await anfDbContext.ComicChapters.Where(x => x.TargetUrl == context.Url)
                        .Take(1)
                        .UpdateFromQueryAsync(x => new KvComicChapter
                        {
                            Pages = pageJson,
                            Title = context.Value.Title,
                            UpdateTime = now
                        });
                }
            }
        }

        public async Task DoneFetchEntityAsync(IValueResourceMonitorContext<AnfComicEntityTruck> context)
        {
            if (context.ProviderFetcher!=this)
            {
                var val = context.Value;
                var now = DateTime.Now.Ticks;
                var upRes = await anfDbContext.ComicEntities.AsNoTracking()
                    .Where(x => x.ComicUrl == context.Url)
                    .Take(1)
                    .UpdateFromQueryAsync(x => new KvComicEntity
                    {
                        ComicUrl = val.ComicUrl,
                        Name = val.Name,
                        Descript = val.Descript,
                        ImageUrl = val.ImageUrl,
                        UpdateTime = now
                    });
                if (upRes != 0)
                {
                    var includeUrls = val.Chapters.Select(x => x.TargetUrl).Distinct().ToArray();
                    var query = anfDbContext.ComicChapters.AsNoTracking();
                    if (includeUrls.Length > 50)
                    {
                        var urlEntity = includeUrls.Select(x => new { TargetUrl=x }).ToArray();
                        query = query.WhereBulkContains(urlEntity, nameof(KvComicChapter.TargetUrl));
                    }
                    else
                    {
                        query = query.Where(x => includeUrls.Contains(x.TargetUrl));
                    }
                    var exists = await query.Select(x=>x.TargetUrl).ToArrayAsync();
                    var existsHash = new HashSet<string>(exists);
                    var notExists = val.Chapters.Where(x => !existsHash.Contains(x.TargetUrl)).ToArray();
                    if (notExists.Length != 0)
                    {
                        var id =await anfDbContext.ComicEntities.AsNoTracking()
                            .Where(x => x.ComicUrl == context.Url)
                            .Select(x => x.Id)
                            .FirstOrDefaultAsync();
                        var chps = notExists.Select(x => new KvComicChapter
                        {
                            EnitityId = id,
                            CreateTime = now,
                            Title = x.Title,
                            TargetUrl = x.TargetUrl,
                        });
                        await anfDbContext.ComicChapters.BulkInsertAsync(chps);
                    }
                }
                else
                {
                    var entity = new KvComicEntity
                    {
                        ComicUrl = context.Value.ComicUrl,
                        Descript = context.Value.Descript,
                        ImageUrl = context.Value.ImageUrl,
                        Name = context.Value.Name,
                        UpdateTime = now,
                        CreateTime = now,
                        Chapters = context.Value.Chapters.Select(x => new KvComicChapter
                        {
                            CreateTime = now,
                            Title = x.Title,
                        }).ToArray(),
                    };
                    await anfDbContext.ComicEntities.SingleInsertAsync(entity);
                }
            }
        }

        public async Task<WithPageChapter> FetchChapterAsync(IResourceFetchContext context)
        {
            var data = await anfDbContext.ComicChapters.AsNoTracking()
                .Where(x => x.TargetUrl == context.Url)
                .FirstOrDefaultAsync();
            if (data is null)
            {
                return null;
            }
            var now = DateTime.Now.Ticks;
            var needUpdate = (now - data.UpdateTime) >= fetchOptions.Value.DataTimeout.Ticks;
            if (needUpdate)
            {
                return null;
            }
            var chapters = JsonSerializer.Deserialize<ComicPage[]>(data.Pages);
            return new WithPageChapter
            {
                Pages = chapters,
                CreateTime = data.CreateTime,
                RefCount = data.RefCount,
                TargetUrl = data.TargetUrl,
                Title = data.Title,
                UpdateTime = data.UpdateTime,
            };
        }

        public async Task<AnfComicEntityTruck> FetchEntityAsync(IResourceFetchContext context)
        {
            var data = await anfDbContext.ComicEntities.AsNoTracking()
                .Include(x=>x.Chapters)
                .Where(x => x.ComicUrl == context.Url)
                .FirstOrDefaultAsync();
            if (data is null)
            {
                return null;
            }
            var now = DateTime.Now.Ticks;
            var needUpdate = (now - data.UpdateTime) >= fetchOptions.Value.DataTimeout.Ticks;
            if (needUpdate)
            {
                return null;
            }
            return new AnfComicEntityTruck
            {
                CreateTime = data.CreateTime,
                RefCount = data.RefCount,
                Chapters = data.Chapters.ToArray(),
                Name = data.Name,
                ImageUrl = data.ImageUrl,
                ComicUrl = data.ComicUrl,
                UpdateTime = data.UpdateTime,
            };
        }
    }
}
