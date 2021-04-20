using GalaSoft.MvvmLight.Command;
using Anf.Easy.Visiting;
using Microsoft.IO;
using System.Net.Http;
using Anf.Platform.Services;
using System.ComponentModel;
using Anf.Platform.Models;

namespace Anf.ViewModels
{
    public class StoreBoxVisitingViewModel<TResource, TImage, TStoreBox> : VisitingViewModel<TResource, TImage>
           where TStoreBox : ComicStoreBox
    {
        public StoreBoxVisitingViewModel(IComicVisiting<TResource> visiting = null) : base(visiting)
        {
        }

        public StoreBoxVisitingViewModel(IComicVisiting<TResource> visiting, HttpClient httpClient, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IStreamImageConverter<TImage> streamImageConverter) : base(visiting, httpClient, recyclableMemoryStreamManager, streamImageConverter)
        {
        }

        private ComicStoreBox storeBox;
        private bool hasStoreBox;

        public bool SuperFavorite
        {
            get => HasStoreBox && StoreBox.AttackModel.SuperFavorite;
            private set
            {
                ToggleSuperFavorite();
            }
        }

        public bool HasStoreBox
        {
            get { return hasStoreBox; }
            private set => Set(ref hasStoreBox, value);
        }

        public ComicStoreBox StoreBox
        {
            get { return storeBox; }
            private set
            {
                if (storeBox != null)
                {
                    storeBox.PropertyChanged -= OnStoreBoxPropertyChanged;
                }
                Set(ref storeBox, value);
                if (value != null)
                {
                    value.PropertyChanged -= OnStoreBoxPropertyChanged;
                }
                HasStoreBox = value != null;
            }
        }

        public RelayCommand ToggleStoreCommand { get; protected set; }
        public RelayCommand ToggleSuperFavoriteCommand { get; protected set; }

        public ComicStoreService<TStoreBox> ComicStoreService { get; protected set; }

        private void OnStoreBoxPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var name = nameof(ComicStoreModel.SuperFavorite);
            if (e.PropertyName == name)
            {
                RaisePropertyChanged(name);
            }
        }
        protected override void OnInitedVisiting()
        {
            ComicStoreService = AppEngine.GetRequiredService<ComicStoreService<TStoreBox>>();
            
            ToggleStoreCommand = new RelayCommand(ToggleStore);
            ToggleSuperFavoriteCommand = new RelayCommand(ToggleSuperFavorite);
        }
        protected override void OnInitDone()
        {
            StoreBox = ComicStoreService.GetStoreBox(ComicEntity.ComicUrl);
        }
        public void ToggleSuperFavorite()
        {
            if (!HasStoreBox)
            {
                ToggleStore();
            }
            if (HasStoreBox)
            {
                StoreBox.AttackModel.SuperFavorite
                    = !StoreBox.AttackModel.SuperFavorite;
            }
        }


        public void ToggleStore()
        {
            if (HasStoreBox)
            {
                ComicStoreService.Remove(ComicEntity.ComicUrl);
            }
            else
            {
                ComicStoreService.Store(ComicEntity);
            }
        }
    }
}
