using Kw.Comic.Engine;
using Kw.Comic.Results;
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
        private readonly VisitService visitService;

        public VisitingController(VisitService visitService)
        {
            this.visitService = visitService;
        }
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<Position>), 200)]
        public IActionResult GetStatus()
        {
            var tsk = Startup.DownloadManager.Position;
            var pos = new Position { Current = tsk.CurrentCount, Total = tsk.TaskCount };
            var res = new EntityResult<Position>
            {
                Data = pos
            };
            return Ok(res);
        }
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ComicEntity>), 200)]
        public async Task<IActionResult> AddDownload([FromQuery]string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest(address);
            }
            var task = await Startup.DownloadManager.AddAsync(address);
            var res = new EntityResult<ComicEntity>
            {
                Data = task?.Link.Request.Entity
            };
            return Ok(res);
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ComicEntity>), 200)]
        public IActionResult GetCurrentComic()
        {
            var entity = Startup.DownloadManager.TaskDispatch.Current?.Link.Request?.Entity;
            var res = new EntityResult<ComicEntity>
            {
                Data = entity
            };
            return Ok(res);
        }
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ComicEntity[]>), 200)]
        public IActionResult GetUnComplatedTask()
        {
            var entities = Startup.DownloadManager.Tasks
                .Select(x=>x.Link.Request.Entity)
                .ToArray();
            var res = new EntityResult<ComicEntity[]>
            {
                Data = entities
            };
            return Ok(res);
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ComicEntity>), 200)]
        public async Task<IActionResult> GetComic(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest();
            }
            var visit = await visitService.GetVisitingAsync(address);
            var res = new EntityResult<ComicEntity>
            {
                Data = visit?.Visiting?.Entity
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
            var visit = await visitService.GetVisitingAsync(address);
            var res = new EntityResult<ChapterWithPage>();
            if (visit != null)
            {
                var mgr = await visit.Visiting.GetChapterManagerAsync(index);
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
            var visit = await visitService.GetVisitingAsync(address);
            if (visit != null)
            {
                var mgr = await visit.Visiting.GetChapterManagerAsync(chapterIndex);
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
