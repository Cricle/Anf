using Kw.Comic.Engine;
using System.IO;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf.Managers
{
    public class PhysicalPage : PhysicalComicPart
    {
        public PhysicalPage(FileInfo file, ComicPage page, PhysicalComicInfo info)
            : base(info.Info)
        {
            Page = page;
            PhysicalComic = info;
            File = file;
        }

        public ComicPage Page { get; }

        public PhysicalComicInfo PhysicalComic { get; }

        public FileInfo File { get; set; }

        public void Update(Stream stream)
        {
            using (var fs = File.Open(FileMode.Create))
            {
                stream.CopyTo(fs);
            }
        }
        public async Task UpdateAsync(Stream stream)
        {
            using (var fs = File.Open(FileMode.Create))
            {
                await stream.CopyToAsync(fs);
            }
        }
    }
}
