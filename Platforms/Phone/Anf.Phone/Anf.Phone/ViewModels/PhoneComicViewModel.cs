using Anf.Easy;
using Anf.Phone.Models;
using Anf.Platform;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace Anf.Phone.ViewModels
{
    public class PhoneComicViewModel : PhoneStorableComicSnapshotInfo, IDisposable
    {
        public PhoneComicViewModel(ComicSnapshot snapshot)
            : base(snapshot)
        {
            SaveLogoImageCommand = new RelayCommand(SaveLogoImage);
            InitLogoImage();
        }
        public PhoneComicViewModel(ComicSnapshot snapshot, ImageSource logoImage)
            : base(snapshot)
        {
            LogoImage = logoImage;
            SaveLogoImageCommand = new RelayCommand(SaveLogoImage);
        }
        private Stream logoStream;
        private ImageSource logoImage;

        public ImageSource LogoImage
        {
            get { return logoImage; }
            private set => Set(ref logoImage, value);
        }
        public RelayCommand SaveLogoImageCommand { get; }
        public async void SaveLogoImage()
        {
            if (logoStream != null)
            {
                //var name = $"{PathHelper.EnsureName(Snapshot.Name)}.jpg";
                await logoStream.PickSaveAsync();
            }
        }
        private async void InitLogoImage()
        {
            try
            {
                logoStream?.Dispose();
                logoStream= await StoreFetchHelper.GetOrFromCacheAsync(Snapshot.ImageUri,async()=> 
                {
                    var rep = await AppEngine.GetRequiredService<HttpClient>().GetAsync(Snapshot.ImageUri);
                    return await rep.Content.ReadAsStreamAsync();
                });
                var conv = AppEngine.GetRequiredService<IStreamImageConverter<ImageSource>>();
                LogoImage =await conv.ToImageAsync(logoStream); 
            }
            catch (Exception) { }
        }

        public void Dispose()
        {
            logoStream?.Dispose();
        }
    }
}
