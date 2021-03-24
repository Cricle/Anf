using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy;
using Kw.Comic.Engine.Easy.Downloading;
using Kw.Comic.Web.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Web.Services
{
    internal class RecordDownloadCenter : DownloadCenter, IRecordDownloadCenter, IDisposable
    {
        private readonly EnginePicker enginePicker;
        private readonly ConcurrentDictionary<string, ProcessInfo> records;
        private readonly ConcurrentDictionary<DownloadTask, ProcessInfo> taskToRecords;
        private readonly ComicHubVisitor comicHubVisitor;
        private readonly ComicDetailCacher comicDetailCacher;
        private readonly IServiceProvider serviceProvider;

        public IEnumerable<ProcessInfo> ProcessInfos => records.Values;

        public RecordDownloadCenter(
            IServiceProvider serviceProvider,
            IDownloadManager downloadTasks,
            IComicSaver saver,
            ComicHubVisitor comicHubVisitor,
            ComicDetailCacher comicDetailCacher,
            EnginePicker enginePicker)
            : base(serviceProvider, downloadTasks, saver)
        {
            this.enginePicker = enginePicker;
            this.comicDetailCacher = comicDetailCacher;
            this.serviceProvider = serviceProvider;
            this.comicHubVisitor = comicHubVisitor;

            Added += OnAdded;
            Done += OnDone;
            Cleared += OnCleared;
            Removed += OnRemoved;
            Seeked += OnSeeked;
            MovedNext += OnMovedNext;

            taskToRecords = new ConcurrentDictionary<DownloadTask, ProcessInfo>();
            records = new ConcurrentDictionary<string, ProcessInfo>();
        }

        public async Task AddOrFromCacheAsync(string address)
        {
            var detail = comicDetailCacher.GetDetail(address);
            if (detail!=null)
            {
                serviceProvider.LoadDownloadAsync(address, detail, Saver);
            }
            else
            {
                await AddAsync(address);
            }
        }

        private void OnMovedNext(DownloadCenter arg1, DownloadTask arg2, int arg3)
        {
            if (taskToRecords.TryGetValue(arg2, out var info))
            {
                info.Current = arg3;
                _ = comicHubVisitor.SendProcessChangedAsync(info.Sign, arg3, info.Total);
            }
        }

        private void OnSeeked(DownloadCenter arg1, DownloadTask arg2, int arg3)
        {
            if (taskToRecords.TryGetValue(arg2, out var info))
            {
                info.Current = arg3;
                _ = comicHubVisitor.SendProcessChangedAsync(info.Sign, arg3, info.Total);
            }
        }

        private void OnRemoved(DownloadCenter arg1, DownloadBox arg2)
        {
            //records.Remove(arg2.Link.Request.Entity.ComicUrl, out var proc);
            //taskToRecords.Remove(arg2.Task, out var proc2);
            //var sign = (proc ?? proc2)?.Sign;
            //if (sign != null)
            //{
            //    _ = comicHubVisitor.SendRemovedAsync(sign, false);
            //}
        }

        private void OnCleared(DownloadCenter obj)
        {
            records.Clear();
            taskToRecords.Clear();
            _ = comicHubVisitor.SendClearedAsync();
        }

        private void OnDone(DownloadCenter arg1, DownloadBox arg2)
        {
            //records.Remove(arg2.Link.Request.Entity.ComicUrl, out var proc);
            //taskToRecords.Remove(arg2.Task, out var proc2);
            //var sign = (proc ?? proc2)?.Sign;
            //if (sign != null)
            //{
            //    _ = comicHubVisitor.SendRemovedAsync(sign, false);
            //}
        }

        private void OnAdded(DownloadCenter arg1, DownloadBox arg2)
        {
            var procInfo = new ProcessInfo
            {
                Total = arg2.Task.Max,
                Current = 0,
                Sign = Md5Helper.MakeMd5(arg2.Link.Request.Entity.ComicUrl),
                Detail = arg2.Link.Request.Detail,
                EngineName = enginePicker.GetProviderIdentity(arg2.Address)
            };
            records.TryAdd(arg2.Address, procInfo);
            taskToRecords.TryAdd(arg2.Task, procInfo);
            _ = comicHubVisitor.SendComicEntityAsync(procInfo);
            comicDetailCacher.SetDetail(procInfo.Detail.Entity.ComicUrl, procInfo.Detail);
        }

        public override void Dispose()
        {
            base.Dispose();
            Added -= OnAdded;
            Done -= OnDone;
            Cleared -= OnCleared;
            Removed -= OnRemoved;
            Seeked -= OnSeeked;
            MovedNext -= OnMovedNext;
        }
    }
}
