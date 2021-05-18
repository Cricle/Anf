using Anf.ResourceFetcher.Fetchers;
using Anf.WebService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Anf.ChannelModel.KeyGenerator;
using Anf.ChannelModel.Mongo;
using Anf.Web.Models;
using Anf.ChannelModel.Results;

namespace Anf.Web.Controllers
{
    [ApiController]
    [Route(AnfConst.ApiPrefx + "[controller]")]
    public class ReadingController : ControllerBase
    {
        private static readonly TimeSpan CacheTime = TimeSpan.FromMinutes(5);

        private const string EntityKey = "Anf.Web.Controllers.ReadingController.Entity";
        private const string ChapterKey = "Anf.Web.Controllers.ReadingController.Chapter";

        private readonly ComicRankService comicRankService;
        private readonly IRootFetcher rootFetcher;
        private readonly IMemoryCache memoryCache;

        public ReadingController(ComicRankService comicRankService,
            IRootFetcher rootFetcher,
            IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.comicRankService = comicRankService;
            this.rootFetcher = rootFetcher;
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(WithPageChapter), 200)]
        public async Task<IActionResult> GetChapter([FromQuery] string url, [FromQuery] string entityUrl)
        {
            var key = RedisKeyGenerator.Concat(ChapterKey, url);
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
            var key = RedisKeyGenerator.Concat(EntityKey, url);
            var res = memoryCache.Get<AnfComicEntityTruck>(key);
            if (res != null)
            {
                await comicRankService.AddScopeAsync(url);
                return Ok(res);
            }
            res = await rootFetcher.FetchEntityAsync(url);
            if (res != null)
            {
                await comicRankService.AddScopeAsync(url);
                memoryCache.Set(key, res, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = CacheTime
                });
            }
            return Ok(res);
        }
    }
}
