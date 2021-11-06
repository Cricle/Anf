using Anf.Easy.Visiting;
using Microsoft.IO;
using System.Net.Http;
using Anf.Platform.Services;
using System.ComponentModel;
using Anf.Platform.Models;
using System;
using Microsoft.Toolkit.Mvvm.Input;

namespace Anf.ViewModels
{
    public class StoreBoxVisitingViewModel<TResource, TImage, TStoreBox> : VisitingViewModel<TResource, TImage>
           where TStoreBox : ComicStoreBox
    {
        public StoreBoxVisitingViewModel(Func<IServiceProvider, IComicVisiting<TImage>> visiting = null)
            : base(visiting)
        {
        }

        public StoreBoxVisitingViewModel(IComicVisiting<TImage> visiting, HttpClient httpClient, RecyclableMemoryStreamManager recyclableMemoryStreamManager, IStreamImageConverter<TImage> streamImageConverter, IObservableCollectionFactory observableCollectionFactory)
            : base(visiting, httpClient, recyclableMemoryStreamManager, streamImageConverter, observableCollectionFactory)
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
            private set => SetProperty(ref hasStoreBox, value);
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
                SetProperty(ref storeBox, value);
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
                OnPropertyChanged(name);
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
            PageCursorMoved += OnStoreBoxVisitingViewModelPageCursorMoved;
        }
        protected override void OnCurrentChaterCursorChanged(IDataCursor<IComicChapterManager<TImage>> cursor)
        {
            var box = StoreBox;
            if (box != null)
            {
                box.AttackModel.CurrentChapter = cursor.CurrentIndex;
            }
        }
        private void OnStoreBoxVisitingViewModelPageCursorMoved(IDataCursor<IComicVisitPage<TImage>> arg1, int arg2)
        {
            var box = StoreBox;
            if (box != null)
            {
                box.AttackModel.CurrentPage = arg2;
            }
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
                StoreBox = null;
            }
            else
            {
                var path=ComicStoreService.Store(ComicEntity);
                StoreBox = ComicStoreService.GetStoreBox(path);
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            PageCursorMoved -= OnStoreBoxVisitingViewModelPageCursorMoved;
        }
    }
}
