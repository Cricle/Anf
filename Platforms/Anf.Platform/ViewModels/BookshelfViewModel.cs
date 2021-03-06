using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Anf.Platform.Services;
using GalaSoft.MvvmLight.Command;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Anf.Platform;

namespace Anf.ViewModels
{
    public abstract class BookshelfViewModel<TStoreBox> : ViewModelBase,IDisposable
        where TStoreBox:ComicStoreBox
    {
        public const int DefaultPageSize = 20;

        public BookshelfViewModel()
        {
            StoreService = AppEngine.GetRequiredService<ComicStoreService<TStoreBox>>();
            observableCollectionFactory = AppEngine.GetRequiredService<IObservableCollectionFactory>();
            NextCommand = new RelayCommand(Next);
            FlushCommand = new RelayCommand(Load);
            RemoveCommand = new RelayCommand(Remove);
            UpdateCommand = new RelayCommand(() => _ = UpdateAsync());
            StoreBoxs = observableCollectionFactory.Create<TStoreBox>();
            Load();
        }
        private readonly IObservableCollectionFactory observableCollectionFactory;

        private IEnumerator<FileInfo> boxEnum;
        private TStoreBox currentBox;
        private int pageSize = DefaultPageSize;
        private bool isLoading;
        private bool endOfFetch;
        private bool isUpdating;
        private int updated;

        public int Updated
        {
            get { return updated; }
            private set
            {
                Set(ref updated, value);
            }
        }

        public bool IsUpdating
        {
            get { return isUpdating; }
            private set => Set(ref isUpdating, value);
        }

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

        public IList<TStoreBox> StoreBoxs { get; }

        public RelayCommand NextCommand { get; }
        public RelayCommand FlushCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand UpdateCommand { get; }


        public void Remove()
        {
            if (currentBox != null)
            {
                StoreService.Remove(CurrentBox.AttackModel.ComicUrl);
                StoreBoxs.Remove(CurrentBox);
                CurrentBox = null;
            }
        }
        public async Task UpdateAsync()
        {
            if (IsUpdating)
            {
                return;
            }
            IsUpdating = true;
            try
            {
                Updated = 0;
                foreach (var item in StoreBoxs)
                {
                    await item.UpdateAsync();
                    Updated++;
                }
            }
            finally
            {
                IsUpdating = false;
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
            StoreBoxs.Remove((TStoreBox)obj);
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
