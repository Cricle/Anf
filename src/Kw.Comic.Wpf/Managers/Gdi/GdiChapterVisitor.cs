using Kw.Comic.Visit;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf.Managers
{
    public class GdiChapterVisitor : ChapterVisitorBase
    {
        public GdiChapterVisitor(ComicPage page, HttpClient httpClient) : base(page, httpClient)
        {
        }
        private Bitmap bitmap;

        public Bitmap Bitmap
        {
            get { return bitmap; }
            private set => RaisePropertyChanged(ref bitmap, value);
        }

        private void OnUnLoad()
        {
            Bitmap?.Dispose();
            Bitmap = null;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnUnLoad();
        }
        protected override Task OnUnLoadAsync()
        {
            OnUnLoad();
            return base.OnUnLoadAsync();
        }

        protected override Task OnLoadAsync(Stream stream)
        {
            OnUnLoad();
            Bitmap = new Bitmap(Image.FromStream(stream));
            return Task.CompletedTask;
        }
    }
}
