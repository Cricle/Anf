using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Anf.ResourceFetcher.Fetchers;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Web;
using System.Net;
using System.Text;
using System.Text.Json;
using Azure.Core.Serialization;

namespace Anf.AzureFunc
{
    public static class Analysis
    {
        [Function("GetPage")]
        public static async Task<HttpResponseData> GetPage([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req)
        {
            var map = HttpUtility.ParseQueryString(req.Url.Query);
            var url = map["url"];
            var rep = req.CreateResponse();
            if (string.IsNullOrEmpty(url))
            {
                rep.StatusCode = HttpStatusCode.BadRequest;
                return rep;
            }
            var engurl = map["engurl"];
            if (string.IsNullOrEmpty(engurl))
            {
                rep.StatusCode = HttpStatusCode.BadRequest;
                return rep;
            }
            var eng = req.FunctionContext.InstanceServices.GetRequiredService<ComicEngine>();
            var type = eng.GetComicSourceProviderType(engurl);
            if (type is null)
            {
                rep.StatusCode = HttpStatusCode.NotFound;
                rep.WriteString(engurl);
                return rep;
            }
            var provider = (IComicSourceProvider)req.FunctionContext.InstanceServices.GetRequiredService(type.ProviderType);
            var res = await provider.GetImageStreamAsync(url);
            rep.Body = res;
            rep.Headers.Add("Content-Type", "application/octet-stream");
            return rep;
        }
        [Function("GetChapters")]
        public static async Task<HttpResponseData> GetChapters([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req)
        {
            var map = HttpUtility.ParseQueryString(req.Url.Query);
            var url = map["url"];
            var rep = req.CreateResponse();
            if (string.IsNullOrEmpty(url))
            {
                rep.StatusCode = HttpStatusCode.BadRequest;
                return rep;
            }
            var engurl = map["engurl"];
            if (string.IsNullOrEmpty(engurl))
            {
                rep.StatusCode = HttpStatusCode.BadRequest;
                return rep;
            }

            var fetcher = req.FunctionContext.InstanceServices.GetRequiredService<IRootFetcher>();
            var chapters = await fetcher.FetchChapterAsync(url, engurl);
            var bytes = JsonSerializer.Serialize(chapters, JsonOptions.DefaultOption);
            rep.WriteString(bytes,JsonOptions.Gb231);
            return rep;
        }
        [Function("GetEntity")]
        public static async Task<HttpResponseData> GetEntity([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req)
        {
            var map = HttpUtility.ParseQueryString(req.Url.Query);
            var url = map["url"];
            var rep = req.CreateResponse();
            if (string.IsNullOrEmpty(url))
            {
                rep.StatusCode = HttpStatusCode.BadRequest;
                return rep;
            }
            var fetcher = req.FunctionContext.InstanceServices.GetRequiredService<IRootFetcher>();
            var chapters = await fetcher.FetchEntityAsync(url);
            var bytes = JsonSerializer.Serialize(chapters, JsonOptions.DefaultOption);
            rep.WriteString(bytes, JsonOptions.Gb231);
            return rep;
        }

        [Function("CanParse")]
        public static HttpResponseData CanParse(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req)
        {
            var eng = req.FunctionContext.InstanceServices.GetRequiredService<ComicEngine>();
            var map = HttpUtility.ParseQueryString(req.Url.Query);
            var rep = req.CreateResponse();
            var url = map["url"];
            if (string.IsNullOrEmpty(url))
            {
                rep.StatusCode = HttpStatusCode.BadRequest;
                return rep;
            }
            var condition = eng.GetComicSourceProviderType(url);
            rep.WriteString(condition.ProviderType.Name, Encoding.UTF8);
            return rep;
        }
    }
}
