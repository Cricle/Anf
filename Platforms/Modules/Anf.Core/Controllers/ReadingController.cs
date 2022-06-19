﻿using Anf.ResourceFetcher.Fetchers;
using Anf.WebService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Anf.ChannelModel.Mongo;
using Anf.Web.Models;
using Anf.ChannelModel.Results;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Anf.Easy.Store;
using SecurityLogin;
using Anf.Statistical;
using Anf.ChannelModel.Entity;
using Anf.ChannelModel;
using System.IO;
using Anf.Core.Finders;
using Ao.Cache;

namespace Anf.Web.Controllers
{
    [ApiController]
    [Route(AnfConst.ApiPrefx + "[controller]")]
    public class ReadingController : ControllerBase
    {
        private static readonly TimeSpan CacheTime = TimeSpan.FromMinutes(5);

        private const string EntityKey = "Anf.Web.Controllers.ReadingController.Entity";
        private const string ChapterKey = "Anf.Web.Controllers.ReadingController.Chapter";
        private const string SearchKey = "Anf.Web.Controllers.ReadingController.Search";

        private readonly ComicImageFinder comicImageFinder;
        private readonly ComicRankService comicRankService;
        private readonly IRootFetcher rootFetcher;
        private readonly IMemoryCache memoryCache;
        private readonly SearchEngine searchEngine;
        private readonly ComicEngine comicEngine;
        private readonly SearchStatisticalService searchStatisticalService;
        private readonly VisitStatisticalService visitStatisticalService;

        public ReadingController(ComicRankService comicRankService,
            IRootFetcher rootFetcher,
            IMemoryCache memoryCache,
            SearchEngine searchEngine,
            ComicEngine comicEngine,
            ComicImageFinder comicImageFinder,
            SearchStatisticalService searchStatisticalService,
            VisitStatisticalService visitStatisticalService)
        {
            this.visitStatisticalService = visitStatisticalService;
            this.searchStatisticalService = searchStatisticalService;
            this.comicImageFinder = comicImageFinder;
            this.comicEngine = comicEngine;
            this.searchEngine = searchEngine;
            this.memoryCache = memoryCache;
            this.comicRankService = comicRankService;
            this.rootFetcher = rootFetcher;
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<string[]>), 200)]
        public IActionResult GetProviders()
        {
            var names = searchEngine.Select(x => x.Name).ToArray();
            var r = new EntityResult<string[]> { Data = names };
            return Ok(r);
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<SearchComicResult>), 200)]
        public async Task<IActionResult> Search([FromQuery] string provider, [FromQuery] string keyword, [FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return base.Problem("The keyword can't be null");
            }
            if (searchEngine.Count == 0)
            {
                return base.Problem("The search engine nothing");
            }
            var prov = searchEngine[0];
            if (!string.IsNullOrEmpty(provider))
            {
                prov = searchEngine.FirstOrDefault(x => x.Name == provider);
                if (prov is null)
                {
                    prov = searchEngine[0];
                }
            }
            var key = KeyGenerator.Concat(SearchKey, provider, keyword);
            var ds = memoryCache.Get(key);
            if (ds != null)
            {
                await comicRankService.IncSearchAsync(keyword, 1);
                return Ok(ds);
            }
            using var scope = searchEngine.ServiceScopeFactory.CreateScope();
            var eng = (ISearchProvider)scope.ServiceProvider.GetService(prov);
            var data = await eng.SearchAsync(keyword, skip, take);
            await comicRankService.IncSearchAsync(keyword, 1);
            await searchStatisticalService.AddAsync(new AnfSearchCount
            {
                Content = keyword,
                IP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                Time = DateTime.Now,
                UserId = HttpContext.Features.Get<UserSnapshot>()?.Id
            });
            var r = new EntityResult<SearchComicResult> { Data = data };
            memoryCache.Set(key, r, new MemoryCacheEntryOptions
            {
                SlidingExpiration = CacheTime
            });
            return Ok(r);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetImage([FromQuery] string entityUrl, [FromQuery] string url)
        {
            var imgRes = await comicImageFinder.FindAsync(new ComicImageIdentity(entityUrl, url));
            return Ok(new EntityResult<string> { Data = imgRes });
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(WithPageChapter), 200)]
        public async Task<IActionResult> GetChapter([FromQuery] string url, [FromQuery] string entityUrl)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest();
            }
            var key = KeyGenerator.Concat(ChapterKey, url);
            var res = memoryCache.Get<WithPageChapter>(key);
            if (res != null)
            {
                return Ok(res);
            }
            res = await rootFetcher.FetchChapterAsync(url, entityUrl);
            if (res != null)
            {
                memoryCache.Set(key, res, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = CacheTime
                });
            }
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(AnfComicEntityTruck), 200)]
        public async Task<IActionResult> GetEntity([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest();
            }
            var key = KeyGenerator.Concat(EntityKey, url);
            var res = memoryCache.Get<AnfComicEntityTruck>(key);
            if (res == null)
            {
                res = await rootFetcher.FetchEntityAsync(url);
                memoryCache.Set(key, res, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = CacheTime
                });
            }
            if (res != null)
            {
                await comicRankService.IncVisitAsync(res.ComicUrl, 1);
                await visitStatisticalService.AddAsync(new AnfVisitCount
                {
                    Address = url,
                    IP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Time = DateTime.Now,
                    UserId = HttpContext.Features.Get<UserSnapshot>()?.Id
                });
            }
            return Ok(res);
        }
    }
}
