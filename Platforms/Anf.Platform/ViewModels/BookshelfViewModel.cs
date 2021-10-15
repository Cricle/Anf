using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using Anf.Platform.Services;
using GalaSoft.MvvmLight.Command;
using System.IO;
using System.Threading.Tasks;

namespace Anf.ViewModels
{
    public abstract class BookshelfViewModel<TStoreBox> : ViewModelBase, IDisposable
        where TStoreBox : ComicStoreBox
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
        private string keyword;
        private TimeSpan updateDelayTime = TimeSpan.FromSeconds(1);
        private TStoreBox updatingBox;

        public TStoreBox UpdateingBox
        {
            get => updatingBox;
            private set => Set(ref updatingBox, value);
        }
        public virtual TimeSpan UpdateDelayTime
        {
            get => updateDelayTime;
            set => Set(ref updateDelayTime, value);
        }
        public string Keyword
        {
            get => keyword;
            set
            {
                Set(ref keyword, value);
                RaiseKeywordChanged(value);
            }
        }
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

        public event Action<BookshelfViewModel<TStoreBox>> ComplatedUpdatedAll;
        public event Action<BookshelfViewModel<TStoreBox>> Loaded;
        public event Action<BookshelfViewModel<TStoreBox>> Nexting;
        public event Action<BookshelfViewModel<TStoreBox>> Nexted;
        public event Action<BookshelfViewModel<TStoreBox>, TStoreBox> StoreBoxRemoved;

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
                    UpdateingBox = item;
                    await item.UpdateAsync();
                    Updated++;
                }
                ComplatedUpdatedAll?.Invoke(this);
            }
            finally
            {
                UpdateingBox = null;
                IsUpdating = false;
            }
        }
        protected virtual async void RaiseKeywordChanged(string keyword)
        {
            await Task.Delay(UpdateDelayTime);
            if (keyword == Keyword)
            {
                Load();
            }
        }
        protected virtual bool BookshelfCondition(string keyword, TStoreBox box)
        {
            return string.IsNullOrEmpty(keyword) || box.AttackModelJson.Contains(keyword);
        }
        public void Next()
        {
            if (!IsLoading && boxEnum != null && !EndOfFetch)
            {
                Nexting?.Invoke(this);
                IsLoading = true;
                try
                {
                    var keyword = Keyword;
                    var t = PageSize;
                    bool ok;
                    while ((ok = boxEnum.MoveNext()) && t-- > 0)
                    {
                        var val = boxEnum.Current;
                        var box = CreateBox(val);
                        if (!BookshelfCondition(keyword, box))
                        {
                            box.Dispose();
                            continue;
                        }
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
                Nexted?.Invoke(this);
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
            Loaded?.Invoke(this);
            Next();
        }

        private void OnItemRemoved(ComicStoreBox obj)
        {
            var box = (TStoreBox)obj;
            StoreBoxs.Remove(box);
            StoreBoxRemoved?.Invoke(this, box);
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
