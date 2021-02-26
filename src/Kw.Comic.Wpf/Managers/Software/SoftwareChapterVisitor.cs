using Kw.Comic.Visit;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#if EnableWin10
using Windows.Storage.Streams;
#endif

namespace Kw.Comic.Wpf.Managers
{
    public class SoftwareChapterVisitor: ChapterVisitorBase
    {
        public SoftwareChapterVisitor(ComicPage page, HttpClient httpClient) : base(page, httpClient)
        {
        }
#if EnableWin10
        private IRandomAccessStream image;

        public IRandomAccessStream Image
        {
            get { return image; }
            private set => RaisePropertyChanged(ref image, value);
        }
#else
        private Stream image;

        public Stream Image
        {
            get { return image; }
            private set => RaisePropertyChanged(ref image, value);
        }
#endif
        public override void Dispose()
        {
            base.Dispose();
            UnLoad();
        }
        private void UnLoad()
        {
            Image?.Dispose();
            Image = null;
        }

        protected override Task OnUnLoadAsync()
        {
            UnLoad();
            return base.OnUnLoadAsync();
        }
        protected override async Task OnLoadAsync(Stream stream)
        {
            if (Image!=null)
            {
                throw new InvalidOperationException("The image is not null, must UnLoad first!");
            }
#if EnableWin10
            var mem = new InMemoryRandomAccessStream();
            await stream.CopyToAsync(mem.AsStream());
#else
            var mem = new MemoryStream();
            await stream.CopyToAsync(mem);
#endif
            Image = mem;
        }
    }
}
