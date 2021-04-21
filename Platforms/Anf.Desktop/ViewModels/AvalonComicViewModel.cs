using Avalonia.Media.Imaging;
using Anf;
using Anf.Easy;
using Anf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Anf.Desktop.Models;
using GalaSoft.MvvmLight.Command;

namespace Anf.Desktop.ViewModels
{
    public class AvalonComicViewModel : AvalonStorableComicSnapshotInfo, IDisposable
    {
        public AvalonComicViewModel(ComicSnapshot snapshot, HttpClient httpClient)
            : base(snapshot)
        {
            this.httpClient = httpClient;
            SaveLogoImageCommand = new RelayCommand(SaveLogoImage);
            InitLogoImage();
        }
        public AvalonComicViewModel(ComicSnapshot snapshot, Bitmap logoImage)
            : base(snapshot)
        {
            LogoImage = logoImage;
            SaveLogoImageCommand = new RelayCommand(SaveLogoImage);
        }
        private readonly HttpClient httpClient;
        private Bitmap logoImage;

        public Bitmap LogoImage
        {
            get { return logoImage; }
            private set => Set(ref logoImage, value);
        }
        public RelayCommand SaveLogoImageCommand { get; }
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
                LogoImage = await CacheFetchHelper.GetAsBitmapOrFromCacheAsync(Snapshot.ImageUri, async () =>
                {
                    var req = await httpClient.GetAsync(Snapshot.ImageUri);
                    return await req.Content.ReadAsStreamAsync();
                });
            }
            catch (Exception) { }
        }

        public void Dispose()
        {
            LogoImage?.Dispose();
        }
    }
}
