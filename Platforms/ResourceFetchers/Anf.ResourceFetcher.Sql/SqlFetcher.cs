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
    public class SqlFetcher : IResourceFetcher
    {
        private readonly IOptions<FetchOptions> fetchOptions;
        private readonly IDbContextTransfer dbContextTransfer;

        public SqlFetcher(IOptions<FetchOptions> fetchOptions,
            IDbContextTransfer dbContextTransfer)
        {
            this.fetchOptions = fetchOptions;
            this.dbContextTransfer = dbContextTransfer;
        }

        public async Task DoneFetchChapterAsync(IValueResourceMonitorContext<WithPageChapter> context)
        {
            if (context.ProviderFetcher != this && !context.FetchContext.IsFromCache)
            {
                var chpSet = dbContextTransfer.GetComicChapterSet();
                var exists = await chpSet.AsNoTracking()
                    .Where(x => x.TargetUrl == context.Url)
                    .AnyAsync();
                var now = DateTime.Now.Ticks;
                if (exists)
                {
                    var pageJson = Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(context.Value.Pages));
                    await chpSet.Where(x => x.TargetUrl == context.Url)
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

        public Task DoneFetchChapterAsync(IValueResourceMonitorContext<WithPageChapter>[] context)
        {
            return Task.WhenAll(context.Select(x => DoneFetchChapterAsync(x)).ToArray());
        }

        public async Task DoneFetchEntityAsync(IValueResourceMonitorContext<AnfComicEntityTruck> context)
        {
            if (context.ProviderFetcher != this && !context.FetchContext.IsFromCache)
            {
                var set = dbContextTransfer.GetComicEntitySet();
                var val = context.Value;
                var now = DateTime.Now.Ticks;
                var upRes = await set.AsNoTracking()
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
                    var chpSet = dbContextTransfer.GetComicChapterSet();
                    var includeUrls = val.Chapters.Select(x => x.TargetUrl).Distinct().ToArray();
                    var query = chpSet.AsNoTracking();
                    if (includeUrls.Length > 50)
                    {
                        var urlEntity = includeUrls.Select(x => new { TargetUrl = x }).ToArray();
                        query = query.WhereBulkContains(urlEntity, nameof(KvComicChapter.TargetUrl));
                    }
                    else
                    {
                        query = query.Where(x => includeUrls.Contains(x.TargetUrl));
                    }
                    var exists = await query.Select(x => x.TargetUrl).ToArrayAsync();
                    var existsHash = new HashSet<string>(exists);
                    var notExists = val.Chapters.Where(x => !existsHash.Contains(x.TargetUrl)).ToArray();
                    if (notExists.Length != 0)
                    {
                        var id = await set.AsNoTracking()
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
                        await chpSet.BulkInsertAsync(chps);
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
                    await set.SingleInsertAsync(entity);
                }
            }
        }

        public Task DoneFetchEntityAsync(IValueResourceMonitorContext<AnfComicEntityTruck>[] context)
        {
            return Task.WhenAll(context.Select(x => DoneFetchEntityAsync(x)).ToArray());
        }

        public async Task<WithPageChapter> FetchChapterAsync(IResourceFetchContext context)
        {
            var datas = await FetchChapterAsync(new[] { context });
            return datas.FirstOrDefault();
        }

        public async Task<WithPageChapter[]> FetchChapterAsync(IResourceFetchContext[] context)
        {
            var urls = context.Select(x => x.Url).ToArray();
            var query = dbContextTransfer.GetComicChapterSet().AsNoTracking();
            query = MakeWhereFilter(query, urls);
            var data = await query
                .ToArrayAsync();
            var now = DateTime.Now.Ticks;
            return data.Where(x => (now - x.UpdateTime) >= fetchOptions.Value.DataTimeout.Ticks)
                .Select(x => new WithPageChapter
                {
                    Pages = JsonSerializer.Deserialize<ComicPage[]>(x.Pages),
                    CreateTime = x.CreateTime,
                    RefCount = x.RefCount,
                    TargetUrl = x.TargetUrl,
                    Title = x.Title,
                    UpdateTime = x.UpdateTime,
                }).ToArray();
        }

        public async Task<AnfComicEntityTruck> FetchEntityAsync(IResourceFetchContext context)
        {
            var datas = await FetchEntityAsync(new[] { context });
            return datas.FirstOrDefault();
        }
        protected virtual IQueryable<KvComicEntity> MakeWhereFilter(IQueryable<KvComicEntity> query,string[] addresses)
        {
            return query.Where(x => addresses.Contains(x.ComicUrl));
        }
        protected virtual IQueryable<KvComicChapter> MakeWhereFilter(IQueryable<KvComicChapter> query, string[] addresses)
        {
            return query.Where(x => addresses.Contains(x.TargetUrl));
        }
        public async Task<AnfComicEntityTruck[]> FetchEntityAsync(IResourceFetchContext[] context)
        {
            var urls = context.Select(x => x.Url).ToArray();
            var query = dbContextTransfer.GetComicEntitySet().AsNoTracking();
            query = MakeWhereFilter(query, urls);
            var data = await query
                .Include(x => x.Chapters)
                .Select(x => new
                {
                    x.ComicUrl,
                    x.CreateTime,
                    x.Descript,
                    x.ImageUrl,
                    x.Name,
                    x.UpdateTime,
                    x.RefCount,
                    Chapters = x.Chapters.Select(y => new ComicChapter
                    {
                        TargetUrl = y.TargetUrl,
                        Title = y.Title,
                    }).ToArray()
                })
                .ToArrayAsync();
            var now = DateTime.Now.Ticks;
            return data.Where(x => (now - x.UpdateTime) >= fetchOptions.Value.DataTimeout.Ticks)
                .Select(x=> new AnfComicEntityTruck
                {
                    CreateTime = x.CreateTime,
                    RefCount = x.RefCount,
                    Chapters = x.Chapters,
                    Name = x.Name,
                    ImageUrl = x.ImageUrl,
                    ComicUrl = x.ComicUrl,
                    UpdateTime = x.UpdateTime,
                }).ToArray();
        }
    }
}