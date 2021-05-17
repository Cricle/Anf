using Anf.ChannelModel;
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

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateBookshelf([FromForm]string name)
        {
            var user = HttpContext.Features.Get<UserSnapshot>();
            await bookshelfService.CreateBookshelfAsync(user.Id,name);
            return Ok();
        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateBookshelfItem([FromForm] long bookshelfId,[FromForm]string address)
        {
            var user = HttpContext.Features.Get<UserSnapshot>();
            await bookshelfService.UpdateBookshelfItemAsync(user.Id, bookshelfId, address, null, null, null);
            return Ok();
        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> DeleteBookshelf([FromForm] long id)
        {
            var user = HttpContext.Features.Get<UserSnapshot>();
            await bookshelfService.DeleteBookshelfAsync(user.Id,id);
            return Ok();
        }
    }
}
