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
        private readonly HttpClient httpClient;

        private readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        private bool isLoaded;


        public ChapterVisitorBase()
        {
        }

        public ChapterVisitorBase(ComicPage page,HttpClient httpClient)
        {
            this.httpClient = httpClient;
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
            Disposed?.Invoke(this);
            locker?.Dispose();
            isLoaded = false;
        }
        ~ChapterVisitorBase()
        {
            Dispose();
        }

        public virtual async Task LoadAsync()
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
                using (var s = await httpClient.GetStreamAsync(Page.TargetUrl))
                {
                    await OnLoadAsync(s);
                }
                Loaded?.Invoke(this);
                IsLoaded = true;
            }
            finally
            {
                locker.Release();
            }
        }
        protected abstract Task OnLoadAsync(Stream stream);
        public override string ToString()
        {
            return $"{{Page:{Page.Name} Loaded:{IsLoaded}}}";
        }

    }
}
