using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy
{
    public class PhysicalFileSaver : IComicSaver
    {
        public string BasePath { get; }
        public FileMode FileMode { get; set; } = FileMode.Create;

        public PhysicalFileSaver(string basePath)
        {
            BasePath = basePath;
        }

        public async Task SaveAsync(ComicDownloadContext context)
        {
            var file = GetFilePath(context.ComicDetail.Entity.Name,
                context.Chapter.Chapter.Title.Trim(),
                context.Page.Name.Trim());
            file = GetDestPath(context, file);
            using (var fs = File.Open(file, FileMode))
            {
                await context.SourceStream.CopyToAsync(fs);
            }
        }

        protected string GetDestPath(ComicDownloadContext context,string path)
        {
            return path;
        }

        public string GetFilePath(string comicName,string folder, string name)
        {
            var cdir = Path.Combine(BasePath, PathHelper.EnsureName(comicName));
            PathHelper.EnsureCreated(cdir);
            var dir = Path.Combine(cdir, folder);
            PathHelper.EnsureCreated(dir);
            var target = Path.Combine(dir, name);
            return target;
        }
    }
}
