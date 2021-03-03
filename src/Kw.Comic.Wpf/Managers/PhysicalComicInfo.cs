using Kw.Comic.Engine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Kw.Comic.Wpf.Managers
{
    public class PhysicalComicInfo : PhysicalComicPart
    {
        public const string Extensions = "comic";

        public PhysicalComicInfo(FileInfo file, ComicEntity info,DirectoryInfo folder)
            : base(info)
        {
            File = file;
            Folder = folder;
        }
        public FileInfo File { get; }

        public DirectoryInfo Folder { get; }

        public void Update()
        {
            var content = JsonConvert.SerializeObject(Info);
            System.IO.File.WriteAllText(File.FullName, content);
        }

        public void Clear()
        {
            Folder.Delete(true);
        }
        public PhysicalChapter GetChapter(ComicChapter chapter)
        {
            Folder.EnsureFolderCreated();
            var name = PathHelper.EnsureName(chapter.Title);
            var directory = Path.Combine(Folder.FullName, name);
            return new PhysicalChapter(this, chapter, new DirectoryInfo(directory));
        }
        public IEnumerable<PhysicalChapter> GetChapters()
        {
            Folder.EnsureFolderCreated();
            foreach (var item in this.Info.Chapters)
            {
                var name = PathHelper.EnsureName(item.Title);
                var directory = Path.Combine(Folder.FullName, name);
                yield return new PhysicalChapter(this, item, new DirectoryInfo(directory));
            }
        }
    }
}
