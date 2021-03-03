using Kw.Comic.Engine;
using Kw.Core.Input;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public abstract class ChapterVisitorBase : ViewModelBase, IResourceVisitor, IDisposable
    {
        private readonly IComicSourceProvider sourceProvider;

        private readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        private bool isLoaded;

        public ChapterVisitorBase(ComicPage page, IComicSourceProvider sourceProvider)
        {
            this.sourceProvider = sourceProvider;
            Page = page;
        }

        public bool IsLoaded
        {
            get => isLoaded;
            private set => RaisePropertyChanged(ref isLoaded, value);
        }

        public ComicPage Page { get; }

        public event Action<ChapterVisitorBase> Loaded;
        public event Action<ChapterVisitorBase> Disposed;

        public virtual void Dispose()
        {
            UnLoadAsync().GetAwaiter().GetResult();
            Disposed?.Invoke(this);
            locker?.Dispose();
            isLoaded = false;
        }
        ~ChapterVisitorBase()
        {
            Dispose();
        }

        public async Task UnLoadAsync()
        {
            if (!IsLoaded)
            {
                return;
            }
            await locker.WaitAsync();
            try
            {
                if (!IsLoaded)
                {
                    return;
                }
                IsLoaded = false;
                await OnUnLoadAsync();
            }
            finally
            {
                locker.Release();
            }
        }
        protected virtual Task OnUnLoadAsync()
        {
            return Task.CompletedTask;
        }
        public async Task LoadFromAsync(Func<Task<Stream>> loadFunc, bool leaveOpen = false)
        {
            if (IsLoaded)
            {
                return;
            }
            await locker.WaitAsync();
            if (IsLoaded)
            {
                locker.Release();
                return;
            }
            try
            {
                if (leaveOpen)
                {
                    var stream = await loadFunc();
                    await OnLoadAsync(stream);
                }
                else
                {
                    using (var stream = await loadFunc())
                    {
                        await OnLoadAsync(stream);
                    }
                }
                
                Loaded?.Invoke(this);
                IsLoaded = true;
            }
            finally
            {
                locker.Release();
            }
        }
        public Task LoadFromStreamAsync(Stream stream)
        {
            return LoadFromAsync(() => Task.FromResult(stream),true);
        }
        public Task LoadFromFileAsync(string filePath)
        {
            return LoadFromAsync(() => Task.FromResult<Stream>(File.OpenRead(filePath)));
        }

        public virtual Task LoadAsync()
        {
            return LoadFromAsync(() => sourceProvider.GetImageStreamAsync(Page.TargetUrl));
        }

        public virtual Stream GetStream()
        {
            return null;
        }

        protected abstract Task OnLoadAsync(Stream stream);
        public override string ToString()
        {
            return $"{{Page:{Page.Name} Loaded:{IsLoaded}}}";
        }

    }
}
