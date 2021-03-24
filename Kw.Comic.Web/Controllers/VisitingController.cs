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
        private readonly EnginePicker enginePicker;
        private readonly SearchEngine searchEngine;
        private readonly IRecordDownloadCenter downloadCenter;

        public VisitingController(IRecordDownloadCenter downloadCenter,
            SearchEngine searchEngine,
            EnginePicker enginePicker)
        {
            this.enginePicker = enginePicker;
            this.searchEngine = searchEngine;
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
        [ProducesResponseType(typeof(EntityResult<ComicSnapshot[]>), 200)]
        public async Task<IActionResult> Search([FromQuery]string keywork)
        {
            var result = await searchEngine.SearchAsync(keywork, 0, 30);
            var res = new EntityResult<ComicSnapshot[]>
            {
                Data = result.Snapshots
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
        public async Task<IActionResult> GetPage([FromQuery]string engineName,[FromQuery] string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest();
            }
            var url=await enginePicker.GetImageStreamAsync(engineName, address);
            if (url != null)
            {
                return PhysicalFile(url, "application/octet-stream");
            }
            return NotFound(address);
        }
    }
}
