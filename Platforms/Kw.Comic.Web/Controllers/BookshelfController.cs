using Kw.Comic.Engine;
using Kw.Comic.Models;
using Kw.Comic.Results;
using Kw.Comic.Services;
using Kw.Comic.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Kw.Comic.Web.Controllers
{
    [ApiController]
    [Route(ComicConst.RouteWithControllerName)]
    public class BookshelfController : ControllerBase
    {
        private readonly VisitingManager visitingManager;
        private readonly IBookshelfService bookshelfService;

        public BookshelfController(IBookshelfService bookshelfService,
            VisitingManager visitingManager)
        {
            this.visitingManager = visitingManager;
            this.bookshelfService = bookshelfService;
        }
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(SetResult<Bookshelf[]>),200)]
        public async Task<IActionResult> Find([FromQuery]int? skip,
            [FromQuery]int? take)
        {
            var val = await bookshelfService.FindBookShelfAsync(skip, take);
            if (val.Datas!=null)
            {
                foreach (var item in val.Datas)
                {
                    var c = await visitingManager.GetVisitingAsync(item.ComicUrl);
                    item.Entity = c.Entity;
                }
            }
            return Ok(val);
        }
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(Result), 200)]
        public async Task<IActionResult> Remove([FromForm]string address)
        {
            if (address is null || !address.IsWebsite())
            {
                return BadRequest();
            }
            var o = await bookshelfService.RemoveAsync(address);
            return Ok(new Result { Code = o == 0 ? 1 : 0 });
        }
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(Result), 200)]
        public async Task<IActionResult> Clear()
        {
            await bookshelfService.ClearAsync();
            return Ok(Result.SucceedResult);
        }
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(EntityResult<Bookshelf>), 200)]
        public async Task<IActionResult> Add([FromForm] string address)
        {
            if (address is null || !address.IsWebsite())
            {
                return BadRequest();
            }
            var book = new Bookshelf
            {
                ComicUrl = address,
                CreateTime = DateTime.Now.Ticks
            };
            await bookshelfService.AddAsync(book);
            return Ok(new EntityResult<Bookshelf> { Data = book });
        }
    }
}
