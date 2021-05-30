using Anf.Platform.Services;
using Anf.Platform.Services.Impl;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Models.Impl
{
    public class WithImageComicStoreBox<TResource, TImage> : ComicStoreBox
    {
        public WithImageComicStoreBox(FileInfo targetFile) : base(targetFile)
        {
            _ = LoadLogoAsync();
        }

        public WithImageComicStoreBox(FileInfo targetFile, ComicStoreModel attackModel) : base(targetFile, attackModel)
        {
            _ = LoadLogoAsync();
        }
        private TImage image;

        public TImage Image
        {
            get { return image; }
            private set => Set(ref image, value);
        }

        public bool DoNotDisposeImage { get; set; }

        public RelayCommand StoreZipCommand { get; protected set; }

        private async Task LoadLogoAsync()
        {
            try
            {
                Image = await StoreFetchHelper.GetOrFromCacheAsync<TResource,TImage>(AttackModel.ImageUrl);
            }
            catch (Exception) { }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (!DoNotDisposeImage && image is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        protected override void CoreRemove()
        {
            var storeSer = AppEngine.GetRequiredService<WithImageComicStoreService<TResource,TImage>>();
            storeSer.Remove(AttackModel.ComicUrl);
        }
    }
}
