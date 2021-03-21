using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy.Downloading;
using Kw.Comic.Results;
using Kw.Comic.Web.Services;
using KwC.Services;
using Microsoft.AspNetCore.Mvc;
using MimeMapping;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KwC.Controllers
{
    [ApiController]
    [Route(ComicConst.RouteWithControllerName)]
    public class VisitingController : ControllerBase
    {
        private readonly VisitingManager visitingManager;
        private readonly IRecordDownloadCenter downloadCenter;

        public VisitingController(IRecordDownloadCenter downloadCenter,
            VisitingManager visitingManager)
        {
            this.visitingManager = visitingManager;
            this.downloadCenter = downloadCenter;
        }
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ComicDetail>), 200)]
        public async Task<IActionResult> AddDownload([FromQuery]string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest(address);
            }
            await downloadCenter.AddOrFromCacheAsync(address);
            downloadCenter.TryGetValue(address, out var box);
            var res = new EntityResult<ComicDetail>
            {
                Data = box?.Link.Request.Detail
            };
            return Ok(res);
        }
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ProcessInfo[]>), 200)]
        public IActionResult GetAll()
        {
            var entities = downloadCenter.ProcessInfos.ToArray();
            var res = new EntityResult<ProcessInfo[]>
            {
                Data = entities
            };
            return Ok(res);
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ComicDetail>), 200)]
        public IActionResult GetComic(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest();
            }
            downloadCenter.TryGetValue(address, out var box);
            var res = new EntityResult<ComicDetail>
            {
                Data = box?.Link.Request.Detail
            };
            return Ok(res);
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ChapterWithPage>), 200)]
        public async Task<IActionResult> GetChapter([FromQuery] string address, [FromQuery] int index)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest();
            }
            var visit = await visitingManager.GetVisitingAsync(address);
            var res = new EntityResult<ChapterWithPage>();
            if (visit != null)
            {
                var mgr = await visit.GetChapterManagerAsync(index);
                res.Data = mgr.ChapterWithPage;
            }
            return Ok(res);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetPage([FromQuery] string address, [FromQuery] int chapterIndex, [FromQuery] int pageIndex)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest();
            }
            var visit = await visitingManager.GetVisitingAsync(address);
            if (visit != null)
            {
                var mgr = await visit.GetChapterManagerAsync(chapterIndex);
                var page=await mgr.GetVisitPageAsync(pageIndex);
                var uri = new Uri(page.Resource);
                if (uri.IsFile)
                {
                    return PhysicalFile(page.Resource, KnownMimeTypes.Img);
                }
                return Redirect(page.Resource);
            }
            return NotFound(new
            {
                Address = address,
                Chapter = chapterIndex,
                Page = pageIndex
            });
        }
    }
}
