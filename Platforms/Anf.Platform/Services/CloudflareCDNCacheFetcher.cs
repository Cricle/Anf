using Anf.Easy.Store;
using Anf.Networks;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Services
{
    public class CloudflareCDNCacheFetcher : IStoreService
    {
       
        private static readonly string ReadWriteUrl = "https://api.cloudflare.com/client/v4/accounts/{0}/storage/kv/namespaces/{1}/values/";
        private static readonly string ExistUrl = $"https://api.cloudflare.com/client/v4/accounts/{0}/storage/kv/namespaces/{1}/keys?limit=1&prefix=";

        private readonly INetworkAdapter networkAdapter;

        private readonly string ScopeReadWriteUrl;
        private readonly string ScopeExistUrl;
        private readonly IReadOnlyDictionary<string, string> scopeHeaders;
        private readonly CloudflareCDNOptions options;
        public CloudflareCDNCacheFetcher(CloudflareCDNOptions options)
        {
#if NETSTANDARD1_4
            this.networkAdapter = new HttpClientAdapter(AppEngine.GetRequiredService<HttpClient>());
#else
            this.networkAdapter = new WebRequestAdapter();
#endif
            this.options = options;
            ScopeReadWriteUrl = string.Format(ReadWriteUrl, options.UserId, options.NameSpaceId);
            ScopeExistUrl = string.Format(ExistUrl, options.UserId, options.NameSpaceId);
            scopeHeaders= new Dictionary<string, string>
            {
                ["X-Auth-Email"] = options.Email,
                ["X-Auth-Key"] = options.Key
            };
        }

        private Task<Stream> FetchAsync(string url,string method,Stream data=null)
        {
            return networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address = url,
                Method = method,
                Headers = scopeHeaders,
                Data = data
            });
        }
        private static string GetKey(string address)
        {
            var identity = Md5Helper.MakeMd5(address);
            var host = UrlHelper.FastGetHost(address);
            return host + "_" + identity;
        }

        public async Task<bool> ExistsAsync(string address)
        {
            var key = GetKey(address);
            var s = await FetchAsync(ScopeExistUrl + key, "get");
            using (var sr=new StreamReader(s))
            {
                var str = sr.ReadToEnd();
                using (var visitor=JsonVisitor.FromString(str))
                {
                    var succeed= string.Equals(visitor["success"]?.ToString(), "true", StringComparison.OrdinalIgnoreCase);
                    if (!succeed)
                    {
                        return false;
                    }
                    var result = visitor["result_info"]?["count"]?.ToString();
                    return result != "0";
                }
            }

        }

        public Task<string> GetPathAsync(string address)
        {
            var key = GetKey(address);
            return Task.FromResult(ReadWriteUrl + key);
        }

        public Task<Stream> GetStreamAsync(string address)
        {
            var key = GetKey(address);
            return FetchAsync(ScopeReadWriteUrl + key, "get");
        }

        public async Task<string> SaveAsync(string address, Stream stream)
        {
            var key = GetKey(address);
            var url = ScopeReadWriteUrl + key;
            if (options.TTLMs!=null)
            {
                url+= $"?expiration_ttl=" + options.TTLMs;
            }
            try
            {
                var s = await FetchAsync(url, "put", stream);
                return url;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Dispose()
        {
        }
    }
}
