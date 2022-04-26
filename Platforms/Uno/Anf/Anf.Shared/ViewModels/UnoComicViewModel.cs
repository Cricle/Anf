using Anf.Easy;
using Anf.Platform;
using Anf.Platform.Models.Impl;
using Anf.Services;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media;

namespace Anf.ViewModels
{
    internal class UnoComicViewModel : WithImageStorableComicSnapshotInfo<ImageBox, ImageBox>, IDisposable
    {
        public UnoComicViewModel(ComicSnapshot snapshot)
            : base(snapshot)
        {
            SaveLogoImageCommand = new RelayCommand(SaveLogoImage);
            InitLogoImage();
        }
        public UnoComicViewModel(ComicSnapshot snapshot, ImageSource logoImage)
            : base(snapshot)
        {
            LogoImage = logoImage;
            SaveLogoImageCommand = new RelayCommand(SaveLogoImage);
        }
        private ImageSource logoImage;

        public ImageSource LogoImage
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
                //await LogoImage.PickSaveAsync(name);
            }
        }
        private async void InitLogoImage()
        {
            try
            {
                LogoImage = (await StoreFetchHelper.GetOrFromCacheAsync<ImageBox, ImageBox>(Snapshot.ImageUri))?.Image;
            }
            catch (Exception) { }
        }

        public void Dispose()
        {
        }
    }
}
