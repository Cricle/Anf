using Anf.Models;
using Anf.Platform.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Platform.Models
{
    public class StorableComicSourceInfo : ComicSourceInfo,IDisposable
    {
        public StorableComicSourceInfo(ComicSnapshot snapshot, 
            ComicSource source, 
            IComicSourceCondition condition,
            ComicStoreBox storeBox) 
            : base(snapshot, source, condition)
        {
            StoreBox = storeBox;
            HasBox = storeBox != null;

            ToggleSuperFavoriteCommand = new RelayCommand(ToggleSuperFavorite);
        }

        public bool HasBox { get; }

        public ComicStoreBox StoreBox { get; }

        public RelayCommand ToggleSuperFavoriteCommand{ get; }

        public void ToggleSuperFavorite()
        {
            if (HasBox)
            {
                StoreBox.AttackModel.SuperFavorite =
                    !StoreBox.AttackModel.SuperFavorite;
            }
        }
        public void Dispose()
        {
            StoreBox?.Dispose();
        }
    }
}
