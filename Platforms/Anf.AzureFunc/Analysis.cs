using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using Anf.AzureFunc.Services;

namespace Anf.AzureFunc
{
    public class Analysis
    {
        private readonly AnalysisService analysisSerivce;
        private readonly ComicEngine eng;
        public Analysis(AnalysisService analysisSerivce, ComicEngine eng)
        {
            this.eng = eng;
            this.analysisSerivce = analysisSerivce;
        }
        [FunctionName("GetPage")]
        public async Task<IActionResult> GetPage([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            var url = req.Query["url"];
            if (url.Count == 0)
            {
                return new BadRequestResult();
            }
            var engurl = req.Query["engurl"];
            if (engurl.Count == 0)
            {
                return new BadRequestResult();
            }
            var res = await analysisSerivce.GetPageAsync(engurl, url);
            var r= new FileStreamResult(res, "application/octet-stream")
            {
                FileDownloadName = Guid.NewGuid().ToString()+".png"
            };
            return r;
        }
        [FunctionName("GetChapters")]
        public async Task<IActionResult> GetChapters([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            var url = req.Query["url"];
            if (url.Count == 0)
            {
                return new BadRequestResult();
            }
            var chapters = await analysisSerivce.GetChaptersAsync(url);
            return new OkObjectResult(chapters);
        }
        [FunctionName("GetEntity")]
        public async Task<IActionResult> GetEntity([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            var url = req.Query["url"];
            if (url.Count == 0)
            {
                return new BadRequestResult();
            }
            var res = await analysisSerivce.GetEntityAsync(url);
            return new OkObjectResult(res);
        }

        [FunctionName("CanParse")]
        public IActionResult CanParse(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            var url = req.Query["url"];
            if (url.Count==0)
            {
                return new BadRequestResult();
            }
            var condition=eng.GetComicSourceProviderType(url);

            return new OkObjectResult(condition != null);
        }
    }
}
