using Avalonia.Media.Imaging;
using Anf;
using Anf.Models;
using Microsoft.IO;
using System;
using System.Net.Http;

namespace Anf.Avalon.Models
{
    public class AvalonComicSnapshotInfo : ComicSnapshotInfo,IDisposable
    {
        public AvalonComicSnapshotInfo(ComicSnapshot snapshot,HttpClient httpClient)
            : base(snapshot)
        {
            this.httpClient = httpClient;
            InitLogoImage();
        }
        private readonly HttpClient httpClient;
        private Bitmap logoImage;

        public Bitmap LogoImage
        {
            get { return logoImage; }
            private set => Set(ref logoImage, value);
        }

        private async void InitLogoImage()
        {
            try
            {

                using (var rep = await httpClient.GetAsync(Snapshot.ImageUri))
                using (var stream = await rep.Content.ReadAsStreamAsync())
                {
                    LogoImage = new Bitmap(stream);
                }
            }
            catch (Exception)
            {

            }
        }

        public void Dispose()
        {
            LogoImage?.Dispose();
        }
    }
}
