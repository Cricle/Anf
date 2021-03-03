using Kw.Comic.Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kw.Comic.Wpf.Managers
{
    public class PhysicalChapter : PhysicalComicPart
    {
        public const string Extensions = "chapter";
        public PhysicalChapter(
            PhysicalComicInfo info,
            ComicChapter chapter,
            DirectoryInfo directory)
            : base(info.Info)
        {
            Chapter = chapter;
            Folder = directory;
            PhysicalComic = info;
        }
        public PhysicalComicInfo PhysicalComic { get; }

        public ComicChapter Chapter { get; }

        public DirectoryInfo Folder { get; }

        public void EnsureCreated()
        {
            Folder.EnsureFolderCreated();
        }
        public void Clear()
        {
            Folder.Delete(true);
        }
        public PhysicalPage GetPage(ComicPage page)
        {
            var pageFn = PathHelper.EnsureName(page.Name);
            var pagePath = Path.Combine(Folder.FullName, pageFn);
            return new PhysicalPage(new FileInfo(pagePath), page,PhysicalComic);
        }
        public IEnumerable<PhysicalPage> GetPages()
        {
            Folder.EnsureFolderCreated();
            var fn = PathHelper.EnsureName($"{Chapter.Title}.{Extensions}");
            var path = Path.Combine(Folder.FullName, fn);
            ComicPage[] content;
            if (!File.Exists(fn))
            {
                content = Array.Empty<ComicPage>();
                File.WriteAllText(path, "[]");
            }
            else
            {
                var str = File.ReadAllText(path);
                content = JsonConvert.DeserializeObject<ComicPage[]>(str);
            }
            foreach (var item in content)
            {
                var pageFn = PathHelper.EnsureName(item.Name);
                var pagePath = Path.Combine(Folder.FullName,pageFn);
                yield return new PhysicalPage(new FileInfo(pagePath), item, PhysicalComic);
            }
        }
    }
}
