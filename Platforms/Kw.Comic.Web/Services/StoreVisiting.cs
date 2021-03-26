using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kw.Comic.Web.Services
{
    public class StoreVisiting : ComicVisiting<string>
    {
        private readonly ComicDetailCacher cacher;

        public StoreVisiting(IServiceProvider host,
            IResourceFactoryCreator<string> resourceFactoryCreator,
            ComicDetailCacher cacher)
            : base(host, resourceFactoryCreator)
        {
            this.cacher = cacher;
        }
        protected override Task<ComicPage[]> GetPagesAsync(ComicChapter chapter)
        {
            var detail = cacher.GetDetail(Address);
            if (detail != null)
            {
                var val = detail.Chapters.FirstOrDefault(x => x.Chapter.TargetUrl == chapter.TargetUrl);
                if (val != null)
                {
                    return Task.FromResult(val.Pages);
                }
            }
            return base.GetPagesAsync(chapter);
        }
        protected override async Task<ComicEntity> MakeEntityAsync(string address)
        {
            var entity = cacher.GetEntity(address);
            if (entity != null)
            {
                return (entity);
            }
            var detail = cacher.GetDetail(address);
            if (detail != null)
            {
                return (detail.Entity);
            }
            var val = await base.MakeEntityAsync(address);
            cacher.SetEntity(address, val);
            return val;
        }
    }
}
