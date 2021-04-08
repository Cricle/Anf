using Anf;
using Anf.Results;
using Anf.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.Controllers
{
    [ApiController]
    [Route(ComicConst.RouteWithControllerName)]
    public class VisitingController : ControllerBase
    {
        private readonly EnginePicker enginePicker;
        private readonly VisitingManager visitingManager;
        private readonly SearchEngine searchEngine;

        public VisitingController(SearchEngine searchEngine,
            EnginePicker enginePicker,
            VisitingManager visitingManager)
        {
            this.visitingManager = visitingManager;
            this.enginePicker = enginePicker;
            this.searchEngine = searchEngine;
        }
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ComicEntityRef>), 200)]
        public async Task<IActionResult> GetComic(string address)
        {
            if (address is null || !address.IsWebsite())
            {
                return BadRequest();
            }
            var visit = await visitingManager.GetVisitingAsync(address);
            var eng = enginePicker.GetProviderIdentity(address);
            var res = new EntityResult<ComicEntityRef>
            {
                Data = new ComicEntityRef { EngineName = eng, Entity = visit.Entity }
            };
            return Ok(res);
        }
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ChapterWithPage>), 200)]
        public async Task<IActionResult> GetChapter(string address, int index)
        {
            if (address is null || !address.IsWebsite())
            {
                return BadRequest();
            }
            var visit = await visitingManager.GetVisitingAsync(address);
            var res = new EntityResult<ChapterWithPage>();
            if (visit is null)
            {
                return Ok(res);
            }
            var chp = await visit.GetChapterManagerAsync(index);
            res.Data = chp.ChapterWithPage;
            return Ok(res);
        }
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ComicSnapshot[]>), 200)]
        public async Task<IActionResult> Search([FromQuery] string keywork)
        {
            var result = await searchEngine.SearchAsync(keywork, 0, 30);
            var res = new EntityResult<ComicSnapshot[]>
            {
                Data = result.Snapshots
            };
            return Ok(res);
        }
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<string>), 200)]
        public IActionResult GetEngine([FromQuery] string address)
        {
            try
            {
                var eng = enginePicker.GetProviderIdentity(address);
                var res = new EntityResult<string> { Data = eng };
                return Ok(res);
            }
            catch (Exception)
            {
                return Ok();
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPage([FromQuery] string engineName, [FromQuery] string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest();
            }
            var url = await enginePicker.GetImageStreamAsync(engineName, address);
            if (url != null)
            {
                return PhysicalFile(url, "application/octet-stream");
            }
            return NotFound(address);
        }
    }
}
