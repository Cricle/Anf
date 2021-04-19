using Anf.Models;
using Anf.Platform.Services;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
            CanStore = condition != null;
            ToggleSuperFavoriteCommand = new RelayCommand(ToggleSuperFavorite);
            AddCommand = new RelayCommand(() => _ = AddAsync());
            RemoveCommand = new RelayCommand(Remove);
            ToggleCommand = new RelayCommand(() => _ = ToggleAsync());
            StoreService = AppEngine.GetRequiredService<ComicStoreService>();
        }
        private bool hasBox;
        private ComicStoreBox storeBox;
        private bool isStoring;

        public bool IsStoring
        {
            get { return isStoring; }
            private set => Set(ref isStoring, value);
        }

        public ComicStoreBox StoreBox
        {
            get { return storeBox; }
            private set
            {
                Set(ref storeBox, value);
                HasBox = value != null;
            }
        }

        public bool HasBox
        {
            get { return hasBox; }
            private set => Set(ref hasBox, value);
        }

        public bool CanStore { get; }

        public ComicStoreService StoreService { get; }

        public RelayCommand ToggleSuperFavoriteCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand ToggleCommand { get; }

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
                return Task.CompletedTask;
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
                        StoreBox = new ComicStoreBox(new FileInfo(path));
                    }
                }
                finally
                {
                    IsStoring = false;
                }
            }
        }
        public void Dispose()
        {
            StoreBox?.Dispose();
        }
    }
}
