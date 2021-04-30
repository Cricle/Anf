using Anf.Easy.Visiting;
using Anf.ViewModels;
using Anf.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.ViewModels
{
    public class WebVisitingViewModel : VisitingViewModel<Stream, Stream>
    {
        private readonly SharedComicVisiting sharedComicVisiting;

        public WebVisitingViewModel(string connectionId)
            : base(null, true)
        {
            ConnectionId = connectionId;
            DoNotDisposeVisiting = true;
            HubContext = provider.GetRequiredService<IHubContext<ReadingHub>>();
        }

        public IHubContext<ReadingHub> HubContext { get; }

        public string ConnectionId { get; }

        protected override Task OnLoadedAsync(string address)
        {
            return base.OnLoadedAsync(address);
        }
        protected override void OnCurrentChaterCursorChanged(IDataCursor<IComicChapterManager<Stream>> cursor)
        {
            base.OnCurrentChaterCursorChanged(cursor);
        }
        protected override async Task<bool> LoadComicAsync(string address)
        {
            visiting = await sharedComicVisiting.JoinAsync(address);
            return visiting != null;
        }
        protected override void OnInitDone()
        {
            base.OnInitDone();
        }
        protected override void OnInitedVisiting()
        {
            base.OnInitedVisiting();
        }
        protected override void OnCurrentPageCursorChanged(IDataCursor<IComicVisitPage<Stream>> cursor)
        {
            base.OnCurrentPageCursorChanged(cursor);
        }
    }
}
