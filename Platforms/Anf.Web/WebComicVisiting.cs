﻿using Anf.Easy.Visiting;
using Anf.ResourceFetcher.Fetchers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web
{
    internal class WebComicVisiting : ComicVisiting<Stream>
    {
        private readonly IRootFetcher rootFetcher;
        public WebComicVisiting(IServiceProvider host, 
            IResourceFactoryCreator<Stream> resourceFactoryCreator,
            IRootFetcher rootFetcher) 
            : base(host, resourceFactoryCreator)
        {
            this.rootFetcher = rootFetcher;
        }
        protected override async Task<ComicPage[]> GetPagesAsync(ComicChapter chapter)
        {
            var entity=await rootFetcher.FetchChapterAsync(chapter.TargetUrl);
            return entity?.Pages;
        }
        protected override async Task<ComicEntity> MakeEntityAsync(string address)
        {
            var entity = await rootFetcher.FetchEntityAsync(address);
            return new ComicEntity
            {
                Chapters = entity.Chapters,
                ComicUrl = entity.ComicUrl,
                Descript = entity.Descript,
                ImageUrl = entity.ImageUrl,
                Name = entity.Name
            };
        }
    }
}
