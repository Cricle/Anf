using Kw.Comic.Blazor.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kw.Comic.Blazor.Server.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AnalysisiController : ControllerBase
    {
        private readonly AnalysisService analysisService;

        public AnalysisiController(AnalysisService analysisService)
        {
            this.analysisService = analysisService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Get(string address)
        {
            var res = await analysisService.GetAsync(address);
            return Ok(res);
        }
        [HttpGet("[action]")]
        public IActionResult CurrentStatus()
        {
            return Ok(analysisService.CurrentResult);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Add(string address)
        {
            var status= await analysisService.AddAsync(address);
            return Ok(status);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Gets(int skip,int take)
        {
            var res = await analysisService.GetsAsync(skip, take);
            return Ok(res);
        }
    }
}
