using Kw.Comic.Engine;
using KwC.Services;
using Microsoft.AspNetCore.SignalR;
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
        public Task SendProcessInfoAsync(ProcessEntity entity, NotifyTypes type)
        {
            var pos = new ProcessInfoResponse
            {
                Type = type,
                Name = entity.Entity.Name,
                ComicUrl = entity.Entity.ComicUrl,
                Chapter = new ProcessItemSnapshot { Name = entity.Chapter.Title, Url = entity.Chapter.TargetUrl },
                Page = new ProcessItemSnapshot { Name = entity.Page.Name, Url = entity.Page.TargetUrl }
            };
            return hubContext.Clients.All.SendAsync(OnReceivedProcessInfo, pos);
        }
    }
}
