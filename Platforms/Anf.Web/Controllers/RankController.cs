using Anf.WebService;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Anf.Web.Models;
using System;
using Anf.ChannelModel.Results;

namespace Anf.Web.Controllers
{
    [ApiController]
    [Route(AnfConst.ApiPrefx + "[controller]")]
    public class RankController : ControllerBase
    {
        private const string Top50Key = "Anf.Web.Controllers.RankController.Top50";
        private readonly ComicRankService comicRankService;
        private readonly IMemoryCache memoryCache;

        public RankController(ComicRankService comicRankService, 
            IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.comicRankService = comicRankService;
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(SetResult<SortedItem>), 200)]
        public async Task <IActionResult> GetHotSearch30()
        {
            var res = await comicRankService.RangeSearchAsync(0, 30);
            var size = await comicRankService.SizeSearchAsync();
            var items = res.Select(x => new SortedItem
            {
                Address = x.Element.ToString(),
                Scope = x.Score
            }).ToArray();
            var ds = new SetResult<SortedItem>
            {
                Skip = 0,
                Take = 50,
                Total = size,
                Datas = items,
            };
            return Ok(ds);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(SetResult<SortedItem>),200)]
        public async Task<IActionResult> GetRank50()
        {
            var ds = memoryCache.Get<SetResult<SortedItem>>(Top50Key);
            if (ds!=null)
            {
                return Ok(ds);
            }
            var res = await comicRankService.RangeVisitAsync(0, 50);
            var size = await comicRankService.SizeVisitAsync();
            var items = res.Select(x => new SortedItem
            {
                Address = x.Element.ToString(),
                Scope = x.Score
            }).ToArray();
            ds = new SetResult<SortedItem>
            {
                Skip = 0,
                Take = 50,
                Total = size,
                Datas = items,
            };
            memoryCache.Set(Top50Key, ds, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(2)
            });
            return Ok(ds);
        }
    }
}
