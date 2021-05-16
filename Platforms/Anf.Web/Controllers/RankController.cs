using Anf.WebService;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Anf.Web.Models;
using System;

namespace Anf.Web.Controllers
{
    [ApiController]
    [Route(AnfConst.ApiPrefx + "[controller]")]
    public class RankController : ControllerBase
    {
        private const string Top50Key = "Anf.Web.Controllers.RankController.Top50";
        private readonly ComicRankService comicRankService;
        private readonly IMemoryCache memoryCache;

        public RankController(ComicRankService comicRankService, IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.comicRankService = comicRankService;
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetRank50()
        {
            var ds = memoryCache.Get<ComicRankItem[]>(Top50Key);
            if (ds!=null)
            {
                return Ok(ds);
            }
            var res = await comicRankService.RangeAsync(0, 50);
            ds = res.Select(x => new ComicRankItem
            {
                Address = x.Element.ToString(),
                Scope = x.Score
            }).ToArray();
            memoryCache.Set(Top50Key, ds, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(5)
            });
            return Ok(ds);
        }
    }
}
