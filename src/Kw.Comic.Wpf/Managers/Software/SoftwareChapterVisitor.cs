using Kw.Comic.Visit;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
#if EnableWin10
using Windows.Storage.Streams;
#elif EnableRecyclableStream
using Microsoft.IO;
#endif

namespace Kw.Comic.Wpf.Managers
{
    public class SoftwareChapterVisitor: ChapterVisitorBase
    {
#if EnableRecyclableStream
        internal static readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager=new RecyclableMemoryStreamManager();
#endif
        public SoftwareChapterVisitor(ComicPage page, HttpClient httpClient) : base(page, httpClient)
        {
        }
        private Stream image;

        public Stream Image
        {
            get { return image; }
            private set => RaisePropertyChanged(ref image, value);
        }

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
        public override Stream GetStream()
        {
            return Image;
        }
        protected override async Task OnLoadAsync(Stream stream)
        {
            if (Image!=null)
            {
                throw new InvalidOperationException("The image is not null, must UnLoad first!");
            }
#if EnableWin10
            var randomStream = new InMemoryRandomAccessStream();
            var mem=randomStream.AsStream();
            await stream.CopyToAsync(mem);
#elif EnableRecyclableStream
            var mem = recyclableMemoryStreamManager.GetStream();
            await stream.CopyToAsync(mem);
#else
            var mem = new MemoryStream();
            await stream.CopyToAsync(mem);
#endif

            Image = mem;
        }
    }
}
