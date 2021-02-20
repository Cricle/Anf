using Kw.Comic.Visit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Kw.Comic.Uwp.Managers
{
    public class UwpChapterVisitor : ChapterVisitorBase
    {
        private InMemoryRandomAccessStream stream;

        public InMemoryRandomAccessStream Stream
        {
            get { return stream; }
            private set => RaisePropertyChanged(ref stream, value);
        }

        public UwpChapterVisitor()
        {
        }

        public UwpChapterVisitor(ComicPage page, HttpClient httpClient) : base(page, httpClient)
        {
        }
        public override void Dispose()
        {
            Stream?.Dispose();
            base.Dispose();
        }
        protected override async Task OnLoadAsync(Stream stream)
        {
            Stream?.Dispose();
            var s = new InMemoryRandomAccessStream();
            await stream.CopyToAsync(s.AsStreamForWrite());
            s.Seek(0);
            Stream = s;
        }
    }
}
