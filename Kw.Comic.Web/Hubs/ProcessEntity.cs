using Kw.Comic;
using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy;
using System;
using System.Linq;

namespace KwC.Hubs
{
    public class ProcessEntity
    {
        public ComicEntity Entity { get; set; }

        public ComicChapter Chapter { get; set; }

        public ComicPage Page { get; set; }

        public static implicit operator ProcessEntity(DownloadListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return new ProcessEntity
            {
                Entity = context.Request.Entity,
                Chapter = context.Chapter,
                Page = context.Page
            };
        }
    }
}
