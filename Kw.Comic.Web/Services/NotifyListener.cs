using Kw.Comic.Engine.Easy;
using KwC.Hubs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace KwC.Services
{
    public class NotifyListener : IDownloadListener
    {
        private readonly IServiceProvider provider;
        private readonly ComicHubVisitor comicHubVisitor;

        public NotifyListener(IServiceProvider provider)
        {
            this.provider = provider;
            comicHubVisitor = this.provider.GetRequiredService<ComicHubVisitor>();
            Startup.InitDone+=()=>
            Startup.DownloadManager.TaskDispatch.CurrentTaskChanged += TaskDispatch_CurrentTaskChanged;
        }

        private async void TaskDispatch_CurrentTaskChanged(DownloadTask obj)
        {
            await comicHubVisitor.SendComicEntityAsync(obj.Link.Request.Entity);
        }

        public Task BeginFetchPageAsync(DownloadListenerContext context)
        {
            return comicHubVisitor.SendProcessInfoAsync(context, NotifyTypes.BeginFetchPage);
        }

        public Task CanceledAsync(DownloadListenerContext context)
        {
            return comicHubVisitor.SendProcessInfoAsync(context, NotifyTypes.Canceled);
        }

        public Task ComplatedSaveAsync(DownloadListenerContext context)
        {
            return comicHubVisitor.SendProcessInfoAsync(context, NotifyTypes.Complated);
        }

        public Task EndFetchPageAsync(DownloadListenerContext context)
        {
            return comicHubVisitor.SendProcessInfoAsync(context, NotifyTypes.EndFetchPage);
        }

        public Task FetchPageExceptionAsync(DownloadExceptionListenerContext context)
        {
            return comicHubVisitor.SendProcessInfoAsync(context, NotifyTypes.FetchPageException);
        }

        public Task NotNeedToSaveAsync(DownloadListenerContext context)
        {
            return comicHubVisitor.SendProcessInfoAsync(context, NotifyTypes.NotNeedToSave);
        }

        public Task ReadyFetchAsync(DownloadListenerContext context)
        {
            return comicHubVisitor.SendProcessInfoAsync(context, NotifyTypes.ReadyFetch);
        }

        public Task ReadySaveAsync(DownloadSaveListenerContext context)
        {
            return comicHubVisitor.SendProcessInfoAsync(context, NotifyTypes.ReadySave);
        }
    }
}
