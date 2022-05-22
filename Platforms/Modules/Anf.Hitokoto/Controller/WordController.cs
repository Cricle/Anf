using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anf.WebService;
using Ao.Cache.MessagePack.Redis;
using Anf.ChannelModel;
using Anf.ChannelModel.Results;
using Anf.Hitokoto.Caching;
using Ao.Cache;

namespace Anf.Hitokoto.Controller
{
    [ApiController]
    [Route(AnfConst.ApiPrefx + "[controller]")]
    public class WordController : ControllerBase
    {
        private readonly MessagePackDataFinder<ulong, WordResponse> wordCacheFinder;
        private readonly RandomWordResultCacheFinder wordResultCacheFinder;

        public WordController(MessagePackDataFinder<ulong, WordResponse> wordCacheFinder, 
            RandomWordResultCacheFinder wordResultCacheFinder)
        {
            this.wordCacheFinder = wordCacheFinder;
            this.wordResultCacheFinder = wordResultCacheFinder;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<RandomWordResult>), 200)]
        public async Task<IActionResult> GetRandom()
        {
            var r = await wordResultCacheFinder.FindInCahceAsync();
            var res = new EntityResult<RandomWordResult>
            {
                Data = r
            };
            return Ok(res);
        }
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(EntityResult<WordResponse>), 200)]
        public async Task<IActionResult> GetWord([FromQuery] ulong wordId)
        {
            var r = await wordCacheFinder.FindAsync(wordId);
            var res = new EntityResult<WordResponse>
            {
                Data = r
            };
            return Ok(res);
        }
    }
}
