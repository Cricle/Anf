using Avalonia.Media.Imaging;
using Anf;
using Anf.Models;
using Microsoft.IO;
using System;
using System.Net.Http;
using Anf.Platform;

namespace Anf.Desktop.Models
{
    public class AvalonComicSnapshotInfo : AvalonStorableComicSnapshotInfo,IDisposable
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
                LogoImage = await StoreFetchHelper.GetOrFromCacheAsync<Bitmap>(Snapshot.ImageUri);
            }
            catch (Exception) 
            {
                //TODO: null img
            }
        }

        public void Dispose()
        {
            LogoImage?.Dispose();
        }
    }
}
