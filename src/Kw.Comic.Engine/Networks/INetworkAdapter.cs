using System;
using System.Collections.Generic;
using System.IO;
#if !NETSTANDARD1_3
using System.Net;
using System.Net.Http;
#else
using System.Net.Http;
#endif
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Networks
{
    public interface INetworkAdapter
    {
        Task<Stream> GetStreamAsync(RequestSettings settings);
    }
    public class HttpClientAdapter : INetworkAdapter
    {
        private readonly HttpClient httpClient;

        public HttpClientAdapter(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Stream> GetStreamAsync(RequestSettings settings)
        {
            var req = new HttpRequestMessage();
            req.RequestUri = new Uri(settings.Address);
            if (settings.Method!=null)
            {
                if (string.Equals("POST",settings.Method, StringComparison.OrdinalIgnoreCase))
                {
                    req.Method = HttpMethod.Post;
                }
                else if (string.Equals("PUT", settings.Method, StringComparison.OrdinalIgnoreCase))
                {
                    req.Method = HttpMethod.Put;
                }
                else
                {
                    req.Method = HttpMethod.Post;
                }
            }
            if (!string.IsNullOrEmpty(settings.ContentType))
            {
                req.Headers.Add("Content-Type", settings.ContentType);
            }
            if (!string.IsNullOrEmpty(settings.Host))
            {
                req.Headers.Host=settings.Host;
            }
            if (!string.IsNullOrEmpty(settings.Referrer))
            {
                req.Headers.Referrer = new Uri(settings.Referrer);
            }
            if (settings.Headers!=null)
            {
                foreach (var item in settings.Headers)
                {
                    req.Headers.Add(item.Key, item.Value);
                }
            }
            if (settings.Data != null)
            {
                req.Content = new StreamContent(settings.Data);
                req.Content.Headers.ContentType = new MediaTypeHeaderValue(settings.ContentType);
            }
            var rep = await httpClient.SendAsync(req);
            return await rep.Content.ReadAsStreamAsync();
        }
    }
#if !NETSTANDARD1_3


    public class WebRequestAdapter : INetworkAdapter
    {
        public async Task<Stream> GetStreamAsync(RequestSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var req = (HttpWebRequest)WebRequest.Create(settings.Address);
            req.Method = "GET";
            req.AllowAutoRedirect = true;
            req.KeepAlive = false;
            if (!string.IsNullOrEmpty(settings.ContentType))
            {
                req.ContentType = settings.ContentType;
            }
            if (!string.IsNullOrEmpty(settings.Host))
            {
                req.Host = settings.Host;
            }
            if (!string.IsNullOrEmpty(settings.Method))
            {
                req.Method = settings.Method;
            }
            if (!string.IsNullOrEmpty(settings.Referrer))
            {
                req.Headers.Add("Referrer", "https://www.dmzj.com/");
            }
            if (settings.Timeout>0)
            {
                req.Timeout = settings.Timeout;
            }
            if (settings.Data!=null)
            {
                using (var s=await req.GetRequestStreamAsync())
                {
                    await settings.Data.CopyToAsync(s);
                }
            }
            if (settings.Headers!=null)
            {
                foreach (var item in settings.Headers)
                {
                    req.Headers.Add(item.Key, item.Value);
                }
            }
            var rep=await req.GetResponseAsync();
            return rep.GetResponseStream();
        }
    }
#endif
}
