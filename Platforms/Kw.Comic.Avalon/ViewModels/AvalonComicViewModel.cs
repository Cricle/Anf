using Avalonia.Media.Imaging;
using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy;
using Kw.Comic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.ViewModels
{
    public class AvalonComicViewModel : ComicSnapshotInfo,IDisposable
    {
        public AvalonComicViewModel(ComicSnapshot snapshot, HttpClient httpClient)
            : base(snapshot)
        {
            this.httpClient = httpClient;
            InitLogoImage();
        }
        public AvalonComicViewModel(ComicSnapshot snapshot, Bitmap logoImage)
            : base(snapshot)
        {
            LogoImage = logoImage;
        }
        private readonly HttpClient httpClient;
        private Bitmap logoImage;

        public Bitmap LogoImage
        {
            get { return logoImage; }
            private set => Set(ref logoImage, value);
        }

        public async void SaveLogoImage()
        {
            if (LogoImage != null)
            {
                var name = $"{PathHelper.EnsureName(Snapshot.Name)}.jpg";
                await LogoImage.PickSaveAsync(name);
            }
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
            if (httpClient !=null)
            {
                LogoImage?.Dispose();
            }
        }
    }
}
