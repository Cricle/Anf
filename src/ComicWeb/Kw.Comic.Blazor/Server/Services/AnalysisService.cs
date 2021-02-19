#pragma warning disable CS0436 // 类型与导入类型冲突
using Google.Protobuf.WellKnownTypes;
using Kw.Comic.Blazor.Server.Caching;
using Kw.Comic.Blazor.Server.Models;
using Kw.Comic.Blazor.Server.Rpc;
using Kw.Comic.Engine;
using LruCacheNet;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Blazor.Server.Services
{
    public class AnalysisService
    {
        public const string FailName = "Unknow";
        public static readonly TimeSpan EmptyDelay = TimeSpan.FromSeconds(10);
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);
        public static readonly TimeSpan CacheTime = TimeSpan.FromDays(1);

        public object SyncRoot { get; } = new object();

        private long total;
        private Task analysisTask;
        private CancellationTokenSource cancellation;
        private readonly HashSet<string> analysisAddress = new HashSet<string>();
        private readonly ConcurrentQueue<string> analysisQuene = new ConcurrentQueue<string>();

        private readonly ConnectionMultiplexer connectionMultiplexer;
        private readonly ComicEngine comicEngine;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly LruCache<string, AnalysisResult> analysisResults;
        private readonly IOptions<AnalysisOptions> analysisOptions;

        public AnalysisService(ComicEngine comicEngine,
            IServiceScopeFactory serviceScopeFactory, 
            IOptions<AnalysisOptions> analysisOptions,
            ConnectionMultiplexer connectionMultiplexer)
        {
            this.connectionMultiplexer = connectionMultiplexer;
            this.comicEngine = comicEngine;
            this.serviceScopeFactory = serviceScopeFactory;
            this.analysisOptions = analysisOptions;
            this.analysisResults = new LruCache<string, AnalysisResult>(500);
        }
        private UpdateProgressReponse currentResult;
        public UpdateProgressReponse CurrentResult => currentResult;
        public long Current => analysisQuene.Count;
        public long Total => total;
        public IEnumerable<AnalysisResult> Results => analysisResults.Values;

        public event Action<AnalysisService> StatusChanged;
        public event Action<AnalysisService, AnalysisResult> Analysised;

        public async Task<AnalysisResult> GetAsync(string address)
        {
            if (!analysisResults.TryGetValue(address, out var res))
            {
                var db = connectionMultiplexer.GetDatabase();
                var key = RedisKeys.AnalysisResultKey + address;
                var r = await db.StringGetAsync(key);
                if (r.HasValue)
                {
                    res = JsonSerializer.Deserialize<AnalysisResult>(r);
                    analysisResults.AddOrUpdate(address, res);
                }
                else
                {
                    key = RedisKeys.AnalysisIdMapKey + address;
                    r = await db.StringGetAsync(key);
                    if (r.HasValue)
                    {
                        r = await db.StringGetAsync(r.ToString());
                        res = JsonSerializer.Deserialize<AnalysisResult>(r);
                        analysisResults.AddOrUpdate(address, res);
                    }
                }
            }

            return res;
        }
        private static long GuidToLong()
        {
            var gb = Guid.NewGuid().ToByteArray();
            var l = BitConverter.ToInt64(gb, 0);
            return l;
        }
        public async Task<AnalysisResult[]> GetsAsync(int skip,int take)
        {
            var ser = connectionMultiplexer.GetServer(connectionMultiplexer.Configuration);
            var db = connectionMultiplexer.GetDatabase();
            var tasks = new List<Task<RedisValue>>(take);
            var batch = db.CreateBatch();
            await foreach (var item in ser.KeysAsync(pattern: RedisKeys.AnalysisResultKey + "*", pageSize: take, cursor: skip))
            {
                var key = item.ToString();
                tasks.Add(batch.StringGetAsync(key));
            }
            batch.Execute();
            await Task.WhenAll(tasks);
            var datas= tasks.Select(x =>
            {
                try
                {
                    return JsonSerializer.Deserialize<AnalysisResult>(x.Result);
                }
                catch { return null; }
            }).Where(x => x != null).ToArray();
            return datas;
        }


        public void Begin()
        {
            if (cancellation!=null)
            {
                cancellation.Cancel();
                cancellation.Dispose();
            }
            cancellation = new CancellationTokenSource();
            analysisTask = Task.Factory.StartNew(AnalysisAsync, cancellation.Token, TaskCreationOptions.LongRunning);
        }

        private async Task AnalysisAsync(object status)
        {
            var db = connectionMultiplexer.GetDatabase();
            var token = (CancellationToken)status;
            while (!token.IsCancellationRequested)
            {
                var current = Current - 1;
                if (analysisQuene.TryDequeue(out var address))
                {
                    var msg = new UpdateProgressReponse();
                    msg.Current = current;
                    msg.Total = total;
                    msg.Operator = UpdateOperators.Complated;
                    msg.Timestamp = new Timestamp();
                    msg.Info = new ComicSnapsnot
                    {
                        Url = address,
                    };
                    try
                    {
                        var ctx = new ComicSourceContext(address);
                        var val = comicEngine.FirstOrDefault(x => x.Condition(ctx));
                        if (val == null)
                        {
                            continue;
                        }
                        using var scope = serviceScopeFactory.CreateScope();
                        var provider = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(val.ProviderType);
                        var entity = await provider.GetChaptersAsync(address);
                        msg.Info.Name = entity.Name;
                        msg.Info.Descript = entity.Descript;
                        var procMsg = msg.Clone();
                        procMsg.Info.Total = entity.Chapters.Length;
                        procMsg.Operator = UpdateOperators.Update;
                        var chapters = new ChapterWithPage[entity.Chapters.Length];
                        for (int i = 0; i < entity.Chapters.Length; i++)
                        {
                            var chapter = entity.Chapters[i];
                            procMsg = procMsg.Clone();
                            procMsg.Info.CurrentName = chapter.Title;
                            procMsg.Info.Current = i;
                            await SendAnalysisingAsync(procMsg);
                            var pages = await provider.GetPagesAsync(chapter.TargetUrl);
                            var cwp = new ChapterWithPage(chapter, pages);
                            chapters[i] = cwp;
                        }
                        var cache = new AnalysisResult
                        {
                            Id= GuidToLong().ToString(),
                            Url = address,
                            Name = entity.Name,
                            ChapterWithPages = chapters,
                            Descript = entity.Descript,
                        };
                        var datas = JsonSerializer.SerializeToUtf8Bytes(cache);
                        var key = RedisKeys.AnalysisResultKey + address;
                        var idKey = RedisKeys.AnalysisIdMapKey + cache.Id;
                        await db.StringSetAsync(idKey, key);
                        await db.StringSetAsync(key, datas);
                        await db.KeyExpireAsync(key, CacheTime);
                        await db.KeyExpireAsync(idKey, CacheTime);
                        msg.Info.Name = entity.Name;
                        try
                        {
                            Analysised?.Invoke(this, cache);
                        }
                        catch (Exception) { }
                    }
                    catch (Exception ex)
                    {
                        msg.Operator = UpdateOperators.Fail;
                        msg.Info.Name = FailName;
                        msg.Info.Descript = ex.Message;
                    }
                    await SendAnalysisingAsync(msg);
                }
                await Task.Delay(EmptyDelay);
            }
        }
        private async Task SendAnalysisingAsync(UpdateProgressReponse reponse)
        {
            currentResult = reponse;
            var writers = ComicAnalysisingService.StreamWriters.Values;
            foreach (var item in writers)
            {
                try
                {
                    await item.WriteAsync(reponse);
                }
                catch (Exception) { }
            }
        }
        public async Task<AddParseStatus> AddAsync(string address)
        {
            if (Current > analysisOptions.Value.Max)
            {
                return AddParseStatus.ToManyJob;
            }
            if (analysisAddress.Contains(address))
            {
                return AddParseStatus.HasSame;
            }
            var key = RedisKeys.AnalysisResultKey + address;
            var db = connectionMultiplexer.GetDatabase();
            var exists = await db.KeyExistsAsync(key);
            if (exists)
            {
                return AddParseStatus.HasSame;
            }
            var ok = Monitor.TryEnter(SyncRoot, Timeout);
            if (!ok)
            {
                return AddParseStatus.Busy;
            }
            try
            {
                var ctx = new ComicSourceContext(address);
                var canParse = comicEngine.Any(x => x.Condition(ctx));
                if (!canParse)
                {
                    return AddParseStatus.NoSupport;
                }
                analysisAddress.Add(address);
                analysisQuene.Enqueue(address);
                total++;
                StatusChanged?.Invoke(this);
            }
            finally
            {
                Monitor.Exit(SyncRoot);
            }
            var writers = ComicAnalysisingService.StreamWriters.Values;
            var msg = new UpdateProgressReponse();
            msg.Current = Current;
            msg.Total = total;
            msg.Timestamp = new Timestamp();
            msg.Operator = UpdateOperators.Add;
            foreach (var item in writers)
            {
                try
                {
                    await item.WriteAsync(msg);
                }
                catch (Exception) { }
            }
            return AddParseStatus.Succeed;
        }
    }
}

#pragma warning restore CS0436 // 类型与导入类型冲突