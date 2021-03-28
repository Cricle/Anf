using System;
using System.IO;
#if !NETSTANDARD1_3
using System.Net.Http;
#else
using System.Net.Http;
#endif
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Networks
{
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
}
