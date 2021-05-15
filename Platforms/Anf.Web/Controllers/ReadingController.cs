using Anf.ChannelModel.Results;
using Anf.Easy.Visiting;
using Anf.ResourceFetcher.Fetchers;
using Anf.WebService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Anf.Web.Controllers
{
    [ApiController]
    [Route(AnfConst.ApiPrefx + "[controller]")]
    public class ReadingController : ControllerBase
    {
        private readonly ComicRankService comicRankService;
        private readonly IRootFetcher rootFetcher;

        public ReadingController(ComicRankService comicRankService, IRootFetcher rootFetcher)
        {
            this.comicRankService = comicRankService;
            this.rootFetcher = rootFetcher;
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetChapter([FromQuery]string url, [FromQuery] string entityUrl)
        {
            var res = await rootFetcher.FetchChapterAsync(url, entityUrl);
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetEntity([FromQuery] string url)
        {
            var res = await rootFetcher.FetchEntityAsync(url);
            if (res != null)
            {
                await comicRankService.AddScopeAsync(url);
            }
            return Ok(res);
        }
    }
}
