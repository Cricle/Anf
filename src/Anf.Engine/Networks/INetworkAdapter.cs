using System;
using System.Collections.Generic;
using System.IO;
#if !NETSTANDARD1_3
using System.Net;
#endif
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Networks
{
    public interface INetworkAdapter
    {
        Task<Stream> GetStreamAsync(RequestSettings settings);
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
            if (!string.IsNullOrEmpty(settings.Accept))
            {
                req.ContentType = settings.Accept;
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
                req.Referer=settings.Referrer;
            }
            if (settings.Timeout > 0)
            {
                req.Timeout = settings.Timeout;
            }
            if (settings.Data != null)
            {
                using (var s = await req.GetRequestStreamAsync())
                {
                    await settings.Data.CopyToAsync(s);
                }
            }
            if (settings.Headers != null)
            {
                foreach (var item in settings.Headers)
                {
                    if (string.Equals("User-Agent", item.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        req.UserAgent = item.Value;
                    }
                    else if (string.Equals("Accept",item.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        req.Accept = item.Value;
                    }
                    else
                    {

                        req.Headers.Add(item.Key, item.Value);
                    }
                }
            }
            var rep = await req.GetResponseAsync();
            return rep.GetResponseStream();
        }
    }
#endif
}
