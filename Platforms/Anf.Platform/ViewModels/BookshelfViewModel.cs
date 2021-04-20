using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Anf.Platform.Services;
using GalaSoft.MvvmLight.Command;
using System.IO;

namespace Anf.ViewModels
{
    public abstract class BookshelfViewModel<TStoreBox> : ViewModelBase,IDisposable
        where TStoreBox:ComicStoreBox
    {
        public const int DefaultPageSize = 20;

        public BookshelfViewModel()
        {
            StoreService = AppEngine.GetRequiredService<ComicStoreService<TStoreBox>>();
            NextCommand = new RelayCommand(Next);
            FlushCommand = new RelayCommand(Load);
            RemoveCommand = new RelayCommand(Remove);
            StoreBoxs = new ObservableCollection<TStoreBox>();
            Load();
        }
        private IEnumerator<FileInfo> boxEnum;
        private TStoreBox currentBox;
        private int pageSize = DefaultPageSize;
        private bool isLoading;
        private bool endOfFetch;

        public bool EndOfFetch
        {
            get { return endOfFetch; }
            private set => Set(ref endOfFetch, value);
        }

        public bool IsLoading
        {
            get { return isLoading; }
            private set => Set(ref isLoading, value);
        }

        public int PageSize
        {
            get { return pageSize; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"PageSize must more than zero");
                }
                Set(ref pageSize, value);
            }
        }

        public TStoreBox CurrentBox
        {
            get { return currentBox; }
            set => Set(ref currentBox, value);
        }

        public ComicStoreService<TStoreBox> StoreService { get; }

        public ObservableCollection<TStoreBox> StoreBoxs { get; }

        public RelayCommand NextCommand { get; }
        public RelayCommand FlushCommand { get; }
        public RelayCommand RemoveCommand { get; }

        public void Remove()
        {
            if (currentBox != null)
            {
                StoreService.Remove(CurrentBox.AttackModel.ComicUrl);
                StoreBoxs.Remove(CurrentBox);
                CurrentBox = null;
            }
        }

        public void Next()
        {
            if (!IsLoading && boxEnum != null && !EndOfFetch)
            {
                IsLoading = true;
                try
                {
                    var t = PageSize;
                    bool ok ;
                    while ((ok = boxEnum.MoveNext()) && t-- > 0)
                    {
                        var val = boxEnum.Current;
                        var box = CreateBox(val);
                        box.Removed += OnItemRemoved;
                        StoreBoxs.Add(box);
                    }
                    if (!ok)
                    {
                        EndOfFetch = true;
                    }
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
        protected abstract TStoreBox CreateBox(FileInfo fileInfo);
        public void Load()
        {
            CurrentBox = null;
            EndOfFetch = false;
            foreach (var item in StoreBoxs)
            {
                item.Removed -= OnItemRemoved;
            }
            StoreBoxs.Clear();
            boxEnum = StoreService.EnumerableModelFiles().GetEnumerator();
            Next();
        }

        private void OnItemRemoved(ComicStoreBox obj)
        {
            StoreBoxs.Remove(CurrentBox);
        }

        public virtual void Dispose()
        {
            foreach (var item in StoreBoxs)
            {
                item.Dispose();
            }
            StoreBoxs.Clear();
        }
    }
}
