using Anf.ChannelModel.Results;
using Anf.Easy.Visiting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.Controllers
{
    [ApiController]
    [Route(AnfConst.ApiPrefx + "[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly SharedComicVisiting sharedComicVisiting;

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<ComicEntity>),200)]
        public async Task<IActionResult> GetChapter(string address)
        {
            var visiting = await sharedComicVisiting.GetAsync(address);
            var res = new EntityResult<ComicEntity>
            {
                Data = visiting.Entity
            };
            return Ok(res);
        }
    }
}
