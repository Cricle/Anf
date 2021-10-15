using Anf.Engine;
using Anf.Platform.Models;
using Anf.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Platform.Services
{
    public abstract class ComicStoreBox : ObservableObject, IDisposable
    {
        public static readonly TimeSpan DefaultLazyTime = TimeSpan.FromSeconds(5);
        protected ComicStoreBox(FileInfo targetFile)
        {
            TargetFile = targetFile ?? throw new ArgumentNullException(nameof(targetFile));
            UpdateModelFromFile();
            Init();
        }
        protected ComicStoreBox(FileInfo targetFile, ComicStoreModel attackModel)
        {
            TargetFile = targetFile ?? throw new ArgumentNullException(nameof(targetFile));
            this.attackModel = attackModel ?? throw new ArgumentNullException(nameof(attackModel));
            AttackModel.PropertyChanged += OnAttackModelPropertyChanged;
            Init();
        }
        private readonly SemaphoreSlim writeLocker = new SemaphoreSlim(1);
        private object updateToken = new object();
        private ComicStoreModel attackModel;
        private bool isSaving;
        private bool isUpdating;

        public bool IsUpdating
        {
            get { return isUpdating; }
            private set => Set(ref isUpdating, value);
        }

        public bool IsSaving
        {
            get { return isSaving; }
            private set => Set(ref isSaving, value);
        }


        public ComicStoreModel AttackModel
        {
            get => attackModel;
            private set => Set(ref attackModel, value);
        }

        public FileInfo TargetFile { get; }
        public string AttackModelJson => JsonHelper.Serialize(AttackModel);

        public RelayCommand ToggleSuperFavoriteCommand { get; protected set; }
        public RelayCommand RemoveCommand { get; protected set; }
        public RelayCommand UpdateCommand { get; protected set; }
        public RelayCommand GoSourceCommand { get; protected set; }

        public event Action<ComicStoreBox> Removed;


        public void ToggleSuperFavorite()
        {
            AttackModel.SuperFavorite
                = !AttackModel.SuperFavorite;
        }

        private void Init()
        {
            ToggleSuperFavoriteCommand = new RelayCommand(ToggleSuperFavorite);
            RemoveCommand = new RelayCommand(Remove);
            UpdateCommand = new RelayCommand(() => _ = UpdateAsync());
            GoSourceCommand = new RelayCommand(GoSource);
        }
        public void Remove()
        {
            CoreRemove();
            Removed?.Invoke(this);
        }
        protected abstract void CoreRemove();
        public Task<bool> LazyWriteAsync()
        {
            return LazyWriteAsync(DefaultLazyTime);
        }

        public void GoSource()
        {
            AppEngine.GetRequiredService<IComicTurnPageService>()
                .GoSource(AttackModel.ComicUrl);
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

                var addr = AttackModel.ComicUrl;
                var eng = AppEngine.GetRequiredService<ComicEngine>();
                var providerType = eng.GetComicSourceProviderType(addr);
                using (var scope = AppEngine.CreateScope())
                {
                    var provider = (IComicSourceProvider)scope.ServiceProvider.GetService(providerType.ProviderType);
                    var entity = await provider.GetChaptersAsync(addr);
                    UpdateEntity(entity);
                }
                await WriteFileAsync();
            }
            finally
            {
                IsUpdating = false;
            }
        }

        public async Task<bool> LazyWriteAsync(TimeSpan delayTime)
        {
            var tk = updateToken;
            await Task.Delay(delayTime);
            var newId = new object();
            if (Interlocked.CompareExchange(ref updateToken, newId, tk) == tk)
            {
                IsSaving = true;
                try
                {
                    await WriteFileAsync();
                }
                finally
                {
                    IsSaving = false;
                }
                return true;
            }
            return false;
        }

        public virtual void WriteFile()
        {
            writeLocker.Wait();
            try
            {
                File.WriteAllText(TargetFile.FullName, AttackModelJson);
            }
            finally
            {
                writeLocker.Release();
            }
        }
        public virtual async Task WriteFileAsync()
        {
            await writeLocker.WaitAsync();
            try
            {

                using (var s = TargetFile.Open(FileMode.Create))
                using (var sr = new StreamWriter(s))
                {
                    await sr.WriteAsync(AttackModelJson);
                }
            }
            finally
            {
                writeLocker.Release();
            }
        }

        public void UpdateEntity(ComicEntity info)
        {
            AttackModel.Chapters = info.Chapters;
            AttackModel.ComicUrl = info.ComicUrl;
            AttackModel.Descript = info.Descript;
            AttackModel.ImageUrl = info.ImageUrl;
        }
        public void UpdateModelFromFile()
        {
            var str = File.ReadAllText(TargetFile.FullName);
            attackModel = JsonHelper.Deserialize<ComicStoreModel>(str);
            attackModel.PropertyChanged += OnAttackModelPropertyChanged;
        }

        public virtual void Dispose()
        {
            AttackModel.PropertyChanged -= OnAttackModelPropertyChanged;
            writeLocker.Wait();
            writeLocker.Dispose();
        }

        private async void OnAttackModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await LazyWriteAsync(DefaultLazyTime);
        }
    }
}
