using StackExchange.Redis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedLockNet;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using SecurityLogin;
using System.Text.Json;
using Structing.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SecurityLogin.AppLogin;
using Anf.ChannelModel.Entity;

namespace Anf.Core.Services
{
    public class AppService : AppService<AnfApp, IAppInfoSnapshot, AppLoginResult, AppServiceOptions>
    {
        public AppService(IKeyGenerator keyGenerator, ITimeHelper timeHelper, ICacheVisitor cacheVisitor)
            : base(keyGenerator, timeHelper, cacheVisitor)
        {
        }

        protected override Task<IAppInfoSnapshot> GetAppInfoSnapshotAsync(string appKey)
        {
            string infoKey = GetInfoKey(appKey);
            return CacheVisitor.GetAsync<IAppInfoSnapshot>(infoKey);
        }
    }
}
