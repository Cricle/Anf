using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy
{
    public class DownloadSaveListenerContext : DownloadListenerContext
    {
        private readonly Stream baseStream;

        public DownloadSaveListenerContext(ComicDownloadRequest request, ComicChapter chapter, ComicPage page, CancellationToken token,
            Stream targetStream) 
            : base(request, chapter, page, token)
        {
            baseStream = targetStream;
        }

        public bool CanCopyStream => baseStream != null;

        private Stream CreateStream()
        {
            baseStream.Seek(0, SeekOrigin.Begin);
            var mem = new MemoryStream((int)baseStream.Length);
            return mem;
        }

        public async Task<Stream> CopyStreamAsync()
        {
            var mem = CreateStream();
            await baseStream.CopyToAsync(mem);
            mem.Seek(0, SeekOrigin.Begin);
            return mem;
        }
        public Stream CopyStream()
        {
            var mem = CreateStream();
            baseStream.CopyTo(mem);
            mem.Seek(0, SeekOrigin.Begin);
            return mem;
        }
    }
}
