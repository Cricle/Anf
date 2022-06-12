using Anf.WebService;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Anf.Web.Models;
using System;
using Anf.ChannelModel.Results;
using Anf.Core.Finders;
using Anf.Core.Models;
using Anf.Core;
using Microsoft.Extensions.Options;

namespace Anf.Web.Controllers
{
    [ApiController]
    [Route(AnfConst.ApiPrefx + "[controller]")]
    public class RankController : ControllerBase
    {
        private readonly ComicRankService comicRankService;
        private readonly VisitRankFinder visitRankFinder;
        private readonly IOptions<VisitRankFetcherOptions> options;

        public RankController(ComicRankService comicRankService, VisitRankFinder visitRankFinder, IOptions<VisitRankFetcherOptions> options)
        {
            this.comicRankService = comicRankService;
            this.visitRankFinder = visitRankFinder;
            this.options = options;
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(SetResult<HotSearchItem>), 200)]
        public async Task <IActionResult> GetHotSearch30()
        {
            var res = await comicRankService.RangeSearchAsync(0, 30);
            var size = await comicRankService.SizeSearchAsync();
            var items = res.Select(x => new HotSearchItem
            {
                Keyword = x.Element.ToString(),
                Scope = x.Score
            }).ToArray();
            var ds = new SetResult<HotSearchItem>
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
        [ProducesResponseType(typeof(EntityResult<RangeVisitEntity>),200)]
        public async Task<IActionResult> GetRank50()
        {
            var res = await visitRankFinder.FindInCahceAsync(options.Value.IntervalCount);
            var ds = new EntityResult<RangeVisitEntity>
            {
                Data = res
            };
            return Ok(ds);
        }
    }
}
