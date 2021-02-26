using Kw.Comic.Visit;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf.Managers
{
    public class SkiaChapterVisitor : ChapterVisitorBase
    {
        public SkiaChapterVisitor(ComicPage page, HttpClient httpClient) : base(page, httpClient)
        {
        }

        private SKImage image;

        public SKImage Image
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
        protected override Task OnLoadAsync(Stream stream)
        {
            Image = SKImage.FromBitmap(SKBitmap.Decode(stream));
            return Task.CompletedTask;
        }
    }
}
