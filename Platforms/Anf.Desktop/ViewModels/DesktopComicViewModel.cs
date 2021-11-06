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
using Anf.Platform;
using Anf.Platform.Models.Impl;
using Microsoft.Toolkit.Mvvm.Input;

namespace Anf.Desktop.ViewModels
{
    public class DesktopComicViewModel : WithImageStorableComicSnapshotInfo<Bitmap, Bitmap>, IDisposable
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
            private set => SetProperty(ref logoImage, value);
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
                LogoImage = await StoreFetchHelper.GetOrFromCacheAsync<Bitmap, Bitmap>(Snapshot.ImageUri);
            }
            catch (Exception) { }
        }

        public void Dispose()
        {
            LogoImage?.Dispose();
        }
    }
}
