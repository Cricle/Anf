using Anf.ChannelModel;
using Anf.ChannelModel.Entity;
using Anf.ChannelModel.Results;
using Anf.WebService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.Controllers
{
    [ApiController]
    [Route(AnfConst.ApiPrefx + "[controller]")]
    public class BookshelfController : ControllerBase
    {
        private readonly BookshelfService bookshelfService;

        public BookshelfController(BookshelfService bookshelfService)
        {
            this.bookshelfService = bookshelfService;
        }

        [Authorize]
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(Result),200)]
        public async Task<IActionResult> CreateBookshelf([FromForm] string name)
        {
            var user = HttpContext.Features.Get<UserSnapshot>();
            await bookshelfService.CreateBookshelfAsync(user.Id, name);
            return Ok(Result.SucceedResult);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<AnfBookshelf>), 200)]
        public async Task<IActionResult> GetBookShelf([FromQuery]long bookshelfId)
        {
            var data = await bookshelfService.GetBookshelfAsync(bookshelfId);
            var res = new EntityResult<AnfBookshelf> { Data = data };
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<AnfBookshelf>), 200)]
        public async Task<IActionResult> GetBookShelfAndItems([FromQuery] long bookshelfId)
        {
            var data = await bookshelfService.GetBookshelfAndItemsAsync(bookshelfId);
            var res = new EntityResult<AnfBookshelf> { Data = data };
            return Ok(res);
        }
        [Authorize]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<AnfBookshelfItem>), 200)]
        public async Task<IActionResult> GetBookshelfItem([FromQuery]long bookshelfId,[FromQuery]string address)
        {
            var user = HttpContext.Features.Get<UserSnapshot>();
            var data = await bookshelfService.GetBookshelfItemAsync(user.Id, bookshelfId, address);
            var r = new EntityResult<AnfBookshelfItem> { Data = data };
            return Ok(r);
        }
        [Authorize]
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(EntityResult<bool>), 200)]
        public async Task<IActionResult> UpdateBookshelfItem([FromForm] long bookshelfId,
            [FromForm] string address,
            [FromForm] int? chapter,
            [FromForm] int? page,
            [FromForm] bool? like)
        {
            var user = HttpContext.Features.Get<UserSnapshot>();
            var res = await bookshelfService.UpdateBookshelfItemAsync(user.Id, bookshelfId, address, chapter, page, like);
            var r = new EntityResult<bool> { Data = res };
            return Ok(r);
        }
        [Authorize]
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(EntityResult<bool>), 200)]
        public async Task<IActionResult> DeleteBookshelf([FromForm] long id)
        {
            var user = HttpContext.Features.Get<UserSnapshot>();
            var res = await bookshelfService.DeleteBookshelfAsync(user.Id, id);
            var r = new EntityResult<bool> { Data = res };
            return Ok(r);
        }
    }
}
