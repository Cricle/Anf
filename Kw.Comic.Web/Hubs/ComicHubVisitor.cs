using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy;
using Kw.Comic.Web.Services;
using KwC.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kw.Comic.Web.Hubs
{
    internal class ComicHubVisitor
    {
        public const string OnReceivedProcessChanged = "OnReceivedProcessChanged";
        public const string OnReceivedEntity = "OnReceivedEntity";
        public const string OnReceivedRemoved = "OnReceivedRemoved";
        public const string OnReceivedCleared= "OnReceivedCleared";

        private readonly IHubContext<ComicHub> hubContext;

        public ComicHubVisitor(IHubContext<ComicHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public Task SendProcessChangedAsync(string sign, int current, int total)
        {
            return hubContext.Clients.All.SendAsync(OnReceivedProcessChanged, sign, current, total);
        }
        public Task SendRemovedAsync(string sign,bool done)
        {
            return hubContext.Clients.All.SendAsync(OnReceivedRemoved, sign, done);
        }
        public Task SendClearedAsync()
        {
            return hubContext.Clients.All.SendAsync(OnReceivedCleared);
        }
        public Task SendComicEntityAsync(ProcessInfo entity)
        {
            return hubContext.Clients.All.SendAsync(OnReceivedEntity, entity);
        }
    }
}
