using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Kw.Comic.Uwp.Managers
{
    [EnableService(ServiceLifetime = ServiceLifetime.Singleton)]
    public class BriefRemarkManager : IDisposable
    {
        private readonly Random random;
        private readonly string[] hosts = new string[]
        {
            "https://v1.hitokoto.cn",
            "https://international.v1.hitokoto.cn"
        };

        private readonly HttpClient httpClient;

        public BriefRemarkManager()
        {
            httpClient = new HttpClient();
            random = new Random();
        }

        public async Task<BriefRemarkEntity> GetBriefRemarkAsync(params BriefRemarkTypes[] types)
        {
            var targetUri = hosts[random.Next(0, 1)] + "?" + string.Join('&', types.Select(x => $"c={(char)(x)}"));
            var rep = await httpClient.GetAsync(new Uri(targetUri));
            var str = await rep.Content.ReadAsStringAsync();
            var entity = JsonSerializer.Deserialize<BriefRemarkEntity>(str);
            return entity;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
