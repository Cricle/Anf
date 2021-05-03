﻿using Anf.Desktop.Services;
using Anf.Easy.Store;
using Anf.Platform;
using Anf.Platform.Books;
using Anf.Platform.Models;
using Anf.Platform.Services;
using Anf.Platform.Stores;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
            _ = LoadLogoAsync();
        }

        public AvalonComicStoreBox(FileInfo targetFile, ComicStoreModel attackModel) : base(targetFile, attackModel)
        {
            _ = LoadLogoAsync();
        }
        private Bitmap image;

        public Bitmap Image
        {
            get { return image; }
            private set => Set(ref image, value);
        }
        public RelayCommand StoreZipCommand { get; protected set; }

        private async Task LoadLogoAsync()
        {
            try
            {
                Image = await StoreFetchHelper.GetOrFromCacheAsync<Bitmap>(AttackModel.ImageUrl);   
            }
            catch (Exception) { }
        }

        public override void Dispose()
        {
            base.Dispose();
            Image?.Dispose();
        }

        protected override void CoreRemove()
        {
            var storeSer = AppEngine.GetRequiredService<DesktopComicStoreService>();
            storeSer.Remove(AttackModel.ComicUrl);
        }
    }
}
