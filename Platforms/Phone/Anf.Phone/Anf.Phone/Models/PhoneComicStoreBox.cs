using Anf.Phone.Services;
using Anf.Platform;
using Anf.Platform.Models;
using Anf.Platform.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Anf.Phone.Models
{
    public class PhoneComicStoreBox : ComicStoreBox
    {
        public PhoneComicStoreBox(FileInfo targetFile) : base(targetFile)
        {
            _ = LoadLogoAsync();
        }

        public PhoneComicStoreBox(FileInfo targetFile, ComicStoreModel attackModel) : base(targetFile, attackModel)
        {
            _ = LoadLogoAsync();
        }
        private ImageSource image;

        public ImageSource Image
        {
            get { return image; }
            private set => Set(ref image, value);
        }
        public RelayCommand StoreZipCommand { get; protected set; }

        private async Task LoadLogoAsync()
        {
            try
            {
                Image = await StoreFetchHelper.GetOrFromCacheAsync<ImageSource>(AttackModel.ImageUrl);
            }
            catch (Exception) { }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (Image is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        protected override void CoreRemove()
        {
            var storeSer = AppEngine.GetRequiredService<PhoneComicStoreService>();
            storeSer.Remove(AttackModel.ComicUrl);
        }
    }
}
