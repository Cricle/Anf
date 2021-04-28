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
using Anf.Platform;

namespace Anf.Desktop.ViewModels
{
    public class DesktopComicViewModel : AvalonStorableComicSnapshotInfo, IDisposable
    {
        public DesktopComicViewModel(ComicSnapshot snapshot)
            : base(snapshot)
        {
            SaveLogoImageCommand = new RelayCommand(SaveLogoImage);
            InitLogoImage();
        }
        public DesktopComicViewModel(ComicSnapshot snapshot, Bitmap logoImage)
            : base(snapshot)
        {
            LogoImage = logoImage;
            SaveLogoImageCommand = new RelayCommand(SaveLogoImage);
        }
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
                LogoImage = await StoreFetchHelper.GetOrFromCacheAsync<Bitmap>(Snapshot.ImageUri);
            }
            catch (Exception) { }
        }

        public void Dispose()
        {
            LogoImage?.Dispose();
        }
    }
}
