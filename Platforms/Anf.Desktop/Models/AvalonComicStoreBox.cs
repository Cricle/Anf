using Anf.Desktop.Services;
using Anf.Platform.Models;
using Anf.Platform.Services;
using Avalonia.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Models
{
    public class AvalonComicStoreBox : ComicStoreBox
    {
        public AvalonComicStoreBox(FileInfo targetFile) : base(targetFile)
        {
            GoSourceCommand = new RelayCommand(GoSource);
            _ = LoadLogoAsync();
        }

        public AvalonComicStoreBox(FileInfo targetFile, ComicStoreModel attackModel) : base(targetFile, attackModel)
        {
            GoSourceCommand = new RelayCommand(GoSource);
            _ = LoadLogoAsync();
        }
        private readonly HttpClient httpClient=AppEngine.GetRequiredService<HttpClient>();
        private Bitmap image;

        public Bitmap Image
        {
            get { return image; }
            private set => Set(ref image, value);
        }

        public RelayCommand GoSourceCommand { get; }

        private async Task LoadLogoAsync()
        {
            Image = await CacheFetchHelper.GetAsBitmapOrFromCacheAsync(this.AttackModel.ImageUrl, DownloadAsync);
            
        }
        private async Task<Stream> DownloadAsync()
        {
            var rep = await httpClient.GetAsync(this.AttackModel.ImageUrl);
            return await rep.Content.ReadAsStreamAsync();
        }
        public void GoSource()
        {
            AppEngine.GetRequiredService<MainNavigationService>()
                .GoSource(AttackModel.ComicUrl);
        }

        public override void Dispose()
        {
            base.Dispose();
            Image?.Dispose();
        }

        protected override void CoreRemove()
        {
            var storeSer = AppEngine.GetRequiredService<AvalonComicStoreService>();
            storeSer.Remove(AttackModel.ComicUrl);
        }
    }
}
