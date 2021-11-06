using Anf.Models;
using Anf.Platform.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Models
{
    public abstract class StorableComicSourceInfo<TStoreBox> : ComicSourceInfo,IDisposable
        where TStoreBox:ComicStoreBox
    {
        public StorableComicSourceInfo(ComicSnapshot snapshot, 
            ComicSource source, 
            IComicSourceCondition condition,
            TStoreBox storeBox) 
            : base(snapshot, source, condition)
        {
            StoreBox = storeBox;
            HasBox = storeBox != null;
            CanStore = condition != null;
            ToggleSuperFavoriteCommand = new RelayCommand(ToggleSuperFavorite);
            AddCommand = new AsyncRelayCommand(AddAsync);
            RemoveCommand = new RelayCommand(Remove);
            ToggleCommand = new AsyncRelayCommand(ToggleAsync);
            StoreService = AppEngine.GetRequiredService<ComicStoreService<TStoreBox>>();
        }
        private bool hasBox;
        private TStoreBox storeBox;
        private bool isStoring;

        public bool IsStoring
        {
            get { return isStoring; }
            private set => SetProperty(ref isStoring, value);
        }

        public TStoreBox StoreBox
        {
            get { return storeBox; }
            private set
            {
                SetProperty(ref storeBox, value);
                HasBox = value != null;
            }
        }

        public bool HasBox
        {
            get { return hasBox; }
            private set => SetProperty(ref hasBox, value);
        }

        public bool CanStore { get; }

        public ComicStoreService<TStoreBox> StoreService { get; }

        public RelayCommand ToggleSuperFavoriteCommand { get; }
        public AsyncRelayCommand AddCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public AsyncRelayCommand ToggleCommand { get; }

        public void ToggleSuperFavorite()
        {
            if (HasBox)
            {
                StoreBox.AttackModel.SuperFavorite =
                    !StoreBox.AttackModel.SuperFavorite;
            }
        }

        public Task ToggleAsync()
        {
            if (HasBox)
            {
                Remove();
                return TaskHelper.GetComplatedTask();
            }
            return AddAsync();
        }

        public void Remove()
        {
            StoreService.Remove(Source.TargetUrl);
            StoreBox = null;
        }

        public async Task AddAsync()
        {
            if (CanStore)
            {
                IsStoring = true;
                try
                {
                    using (var scope = AppEngine.CreateScope())
                    {
                        var provider = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(Condition.ProviderType);
                        var entity = await provider.GetChaptersAsync(Source.TargetUrl);
                        var path=StoreService.Store(entity);
                        StoreBox = CreateBox(new FileInfo(path));
                    }
                }
                finally
                {
                    IsStoring = false;
                }
            }
        }
        protected abstract TStoreBox CreateBox(FileInfo file);
        public void Dispose()
        {
            StoreBox?.Dispose();
        }
    }
}
