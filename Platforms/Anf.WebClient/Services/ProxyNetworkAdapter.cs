using Anf.Networks;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anf.WebClient.Services
{
    internal class ProxyNetworkAdapter : INetworkAdapter
    {
        internal class SendModel
        {
            public string Body { get; set; }

            public string Url { get; set; }

            public string Method { get; set; }

            public IReadOnlyDictionary<string,string> Headers { get; set; }
        }
        private INetworkAdapter GetNetWorkAdatapter()
        {
            return new HttpClientAdapter(AppEngine.GetRequiredService<HttpClient>());
        }
        public async Task<Stream> GetStreamAsync(RequestSettings settings)
        {
            var ada = GetNetWorkAdatapter();
            if (string.Equals(settings.Method,"GET", StringComparison.OrdinalIgnoreCase))
            {
                var url = AnfConst.ProxUrl + $"?url={settings.Address}&method=get";
                if (settings.Headers!=null&&settings.Headers.Count!=0)
                {
                    url += string.Join('&', settings.Headers.Select(x => $"{x.Key}={x.Value}"));
                }
                if (!string.IsNullOrEmpty(settings.Host))
                {
                    url += "&Host=" + settings.Host;
                }
                if (!string.IsNullOrEmpty(settings.Referrer))
                {
                    url += "&Referrer=" + settings.Referrer;
                }
                if (!string.IsNullOrEmpty(settings.Accept))
                {
                    url += "&Accept=" + settings.Accept;
                }
                settings.Headers = null;
                settings.Address = url;
                return await ada.GetStreamAsync(settings);
            }
            else
            {
                var model = new SendModel
                {
                    Url = settings.Address,
                    Headers = settings.Headers,
                    Method = settings.Method,
                };
                Dictionary<string, string> headers = null;
                if (settings.Headers is null)
                {
                    headers = new Dictionary<string, string>();
                }
                else
                {
                    headers = new Dictionary<string, string>(settings.Headers);
                }
                if (!string.IsNullOrEmpty(settings.Host))
                {
                    headers["Host"] = settings.Host;
                }
                if (!string.IsNullOrEmpty(settings.Referrer))
                {
                    headers["Referrer"] = settings.Referrer;
                }
                if (!string.IsNullOrEmpty(settings.Accept))
                {
                    headers["Accept"] = settings.Accept;
                }
                Stream mem = null;
                if (settings.Data!=null)
                {
                    using (var sr=new StreamReader(settings.Data))
                    {
                        model.Body = sr.ReadToEnd();
                    }
                    var memMgr = AppEngine.GetRequiredService<RecyclableMemoryStreamManager>();
                    mem = memMgr.GetStream();
                    var json = JsonSerializer.SerializeToUtf8Bytes(model);
                    mem.Write(json);
                    mem.Seek(0, SeekOrigin.Begin);
                    settings.Data = mem;
                }
                settings.Headers = headers;
                settings.Address = AnfConst.ProxUrl;
                try
                {
                    return await ada.GetStreamAsync(settings);
                }
                finally
                {
                    mem?.Dispose();
                }
            }
        }
    }
}
