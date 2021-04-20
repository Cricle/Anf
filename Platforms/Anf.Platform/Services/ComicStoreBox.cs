using Anf.Platform.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Platform.Services
{
    public abstract class ComicStoreBox : ObservableObject,IDisposable
    {
        public static readonly TimeSpan DefaultLazyTime = TimeSpan.FromSeconds(5);
        public ComicStoreBox(FileInfo targetFile)
        {
            TargetFile = targetFile ?? throw new ArgumentNullException(nameof(targetFile));
            ToggleSuperFavoriteCommand = new RelayCommand(ToggleSuperFavorite);
            RemoveCommand = new RelayCommand(Remove);
            UpdateModelFromFile();
        }
        public ComicStoreBox(FileInfo targetFile, ComicStoreModel attackModel)
        {
            TargetFile = targetFile ?? throw new ArgumentNullException(nameof(targetFile));
            this.attackModel = attackModel ?? throw new ArgumentNullException(nameof(attackModel));
            AttackModel.PropertyChanged += OnAttackModelPropertyChanged;
            ToggleSuperFavoriteCommand = new RelayCommand(ToggleSuperFavorite);
            RemoveCommand = new RelayCommand(Remove);
        }
        private readonly SemaphoreSlim writeLocker = new SemaphoreSlim(1);
        private object updateToken=new object();
        private ComicStoreModel attackModel;

        public FileInfo TargetFile { get; }

        public ComicStoreModel AttackModel
        {
            get => attackModel;
            private set => Set(ref attackModel, value);
        }

        public string AttackModelJson => JsonConvert.SerializeObject(AttackModel);

        public RelayCommand ToggleSuperFavoriteCommand { get; }
        public RelayCommand RemoveCommand { get; }

        public event Action<ComicStoreBox> Removed;

        public void ToggleSuperFavorite()
        {
            AttackModel.SuperFavorite
                = !AttackModel.SuperFavorite;
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
        public async Task<bool> LazyWriteAsync(TimeSpan delayTime)
        {
            var tk = updateToken;
            await Task.Delay(delayTime);
            var newId = new object();
            if (Interlocked.CompareExchange(ref updateToken, newId, tk) == tk)
            {
                await WriteFileAsync();
                return true;
            }
            return false;
        }

        public void WriteFile()
        {
            writeLocker.Wait();
            try
            {
                using (var s = TargetFile.Open(FileMode.Create))
                using (var sr = new StreamWriter(s))
                {
                    sr.Write(AttackModelJson);
                }
            }
            finally
            {
                writeLocker.Release();
            }
        }
        public async Task WriteFileAsync()
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
            AttackModel.Descript= info.Descript;
            AttackModel.ImageUrl = info.ImageUrl;
        }
        public void UpdateModelFromFile()
        {
            var str = File.ReadAllText(TargetFile.FullName);
            attackModel = JsonConvert.DeserializeObject<ComicStoreModel>(str);
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
