using Anf.Easy;
using Anf.Networks;
using Anf.Platform;
using Anf.Platform.Models.Impl;
using GalaSoft.MvvmLight.Command;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Cross.ViewModels
{
    public class MAUIComicViewModel : WithImageStorableComicSnapshotInfo<Stream, ImageSource>,IDisposable
    {
        public MAUIComicViewModel(ComicSnapshot snapshot)
            : base(snapshot)
        {
            SaveLogoImageCommand = new RelayCommand(SaveLogoImage);
            InitLogoImage();
        }
        public MAUIComicViewModel(ComicSnapshot snapshot, ImageSource logoImage)
            : base(snapshot)
        {
            LogoImage = logoImage;
            SaveLogoImageCommand = new RelayCommand(SaveLogoImage);
        }
        private ImageSource logoImage;
        private Stream logoStream;

        public ImageSource LogoImage
        {
            get { return logoImage; }
            private set => Set(ref logoImage, value);
        }
        public RelayCommand SaveLogoImageCommand { get; }
        public async void SaveLogoImage()
        {
            if (LogoImage != null)
            {
                //var name = $"{PathHelper.EnsureName(Snapshot.Name)}.jpg";
                await logoStream.PickSaveAsync();
            }
        }
        private void InitLogoImage()
        {
            try
            {
                logoStream?.Dispose();
                logoImage = ImageSource.FromStream(async tk =>
                  {
                      logoStream = await StoreFetchHelper.GetOrFromCacheAsync(Snapshot.ImageUri, () =>
                      {
                          var networkAdapter = AppEngine.GetRequiredService<INetworkAdapter>();
                          return networkAdapter.GetStreamAsync(new RequestSettings { Address = Snapshot.ImageUri });
                      });
                      return logoStream;
                  });
            }
            catch (Exception) { }
        }

        public void Dispose()
        {
            logoStream?.Dispose();
            logoImage = null;
        }
    }
}
