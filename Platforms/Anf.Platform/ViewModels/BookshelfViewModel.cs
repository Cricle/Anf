using GalaSoft.MvvmLight;
using Anf.Easy.Store;
using Anf.Models;
using Anf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Anf.Platform.Services;
using GalaSoft.MvvmLight.Command;
using Anf.Platform.Models;
using System.IO;

namespace Anf.ViewModels
{
    public class BookshelfViewModel : ViewModelBase
    {
        public const int DefaultPageSize = 20;

        public BookshelfViewModel()
        {
            StoreService = AppEngine.GetRequiredService<ComicStoreService>();
            NextCommand = new RelayCommand(Next);
            FlushCommand = new RelayCommand(Load);
            RemoveCommand = new RelayCommand(Remove);
            StoreBoxs = new ObservableCollection<ComicStoreBox>();
            Load();
        }
        private IEnumerator<FileInfo> boxEnum;
        private ComicStoreBox currentBox;
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

        public ComicStoreBox CurrentBox
        {
            get { return currentBox; }
            set => Set(ref currentBox, value);
        }

        public ComicStoreService StoreService { get; }

        public ObservableCollection<ComicStoreBox> StoreBoxs { get; }

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
                        var box = new ComicStoreBox(val);
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
    }
}
