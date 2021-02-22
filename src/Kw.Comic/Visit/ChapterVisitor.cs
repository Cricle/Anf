using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class ChapterVisitor : ChapterVisitorBase, IStreamVisitor
    {
        private Stream stream;

        public ChapterVisitor(ComicPage page, HttpClient httpClient) : base(page, httpClient)
        {
        }

        public Stream Stream
        {
            get => stream;
            private set => RaisePropertyChanged(ref stream, value);
        }

        public override void Dispose()
        {
            stream?.Dispose();
            base.Dispose();
        }
        protected override async Task OnLoadAsync(Stream stream)
        {
            var mem = new MemoryStream();
            await stream.CopyToAsync(mem);
            mem.Seek(0, SeekOrigin.Begin);
            Stream = mem;
        }
        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
