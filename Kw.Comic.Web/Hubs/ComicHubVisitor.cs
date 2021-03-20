using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy;
using KwC.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KwC.Hubs
{
    internal class ComicHubVisitor
    {
        public const string OnReceivedProcessChanged = "OnReceivedProcessChanged";
        public const string OnReceivedProcessInfo = "OnReceivedProcessInfo";
        public const string OnReceivedEntity= "OnReceivedEntity";

        private readonly IHubContext<ComicHub> hubContext;

        public ComicHubVisitor(IHubContext<ComicHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public Task SendProcessChangedAsync(int current, int total)
        {
            return hubContext.Clients.All.SendAsync(OnReceivedProcessChanged, current,total);
        }
        public Task SendComicEntityAsync(ComicEntity entity)
        {
            return hubContext.Clients.All.SendAsync(OnReceivedEntity, entity);
        }
        public Task SendProcessInfoAsync(DownloadListenerContext entity, NotifyTypes type)
        {
            var pageIndex = -1;
            var pos = 0;
            foreach (var item in entity.Request.DownloadRequests)
            {
                if (item.Chapter.TargetUrl == entity.Chapter.TargetUrl)
                {
                    if (item.Page.TargetUrl == entity.Page.TargetUrl)
                    {
                        pageIndex = pos;
                    }
                    pos++;
                }
            }
            var posx = new ProcessInfoResponse
            {
                Type = type,
                Name = entity.Request.Entity.Name,
                ComicUrl = entity.Request.Entity.ComicUrl,
                Chapter = new ProcessItemSnapshot
                {
                    Name = entity.Chapter.Title,
                    Url = entity.Chapter.TargetUrl,
                    Total = entity.Request.Entity.Chapters.Length,
                    Current = Array.FindIndex(entity.Request.Entity.Chapters, x => x.TargetUrl == entity.Chapter.TargetUrl)
                },
                Page = new ProcessItemSnapshot
                {
                    Name = entity.Page.Name,
                    Url = entity.Page.TargetUrl,
                    Total = pos,
                    Current = pageIndex
                }
            };
            return hubContext.Clients.All.SendAsync(OnReceivedProcessInfo, posx);
        }
    }
}
