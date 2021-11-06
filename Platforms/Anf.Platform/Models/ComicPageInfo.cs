using Anf.Easy.Visiting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Anf.Services;
using System.Diagnostics;
using Anf.Platform;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Anf.Models
{
    public class ComicPageInfo<TResource> : ObservableObject
    {
        private static readonly object SharedObject = new object();
        private IComicVisitPage<TResource> visitPage;
        private bool loading;
        private Exception exception;
        private TResource resource;
        private bool loadSucceed;
        private bool hasException;
        private bool emitLoad;

        public bool EmitLoad
        {
            get { return emitLoad; }
            private set => SetProperty(ref emitLoad, value);
        }

        public bool HasException
        {
            get { return hasException; }
            private set => SetProperty(ref hasException, value);
        }

        public bool LoadSucceed
        {
            get { return loadSucceed; }
            private set
            {
                SetProperty(ref loadSucceed, value);
                if (value)
                {
                    LoadDone?.Invoke(this);
                }
            }
        }

        public TResource Resource
        {
            get { return resource; }
            private set
            {
                SetProperty(ref resource, value);
            }
        }

        public Exception Exception
        {
            get { return exception; }
            private set => SetProperty(ref exception, value);
        }

        public bool Loading
        {
            get { return loading; }
            private set => SetProperty(ref loading, value);
        }

        public IComicVisitPage<TResource> VisitPage
        {
            get => visitPage;
            private set => SetProperty(ref visitPage, value);
        }
        private object locker = SharedObject;
        private Task<IComicVisitPage<TResource>> task;

        public ComicPage Page { get; }

        public ComicPageInfo(PageSlots<TResource> pageSlots, int index)
        {
            PageSlots = pageSlots ?? throw new ArgumentNullException(nameof(pageSlots));
            Index = index;
            Page = PageSlots.ChapterManager.ChapterWithPage.Pages[index];
            PageInfoType = ComicPageInfoTypes.FromLoad;
            Init();
        }
        public ComicPageInfo(IComicVisitPage<TResource> visitPage)
        {
            VisitPage = visitPage;
            Page = visitPage.Page;
            PageInfoType = ComicPageInfoTypes.FromValue;
            Init();
        }

        public ComicPageInfoTypes PageInfoType { get; }

        public PageSlots<TResource> PageSlots { get; }

        public int Index { get; }

        public AsyncRelayCommand LoadCommand { get; protected set; }
        public AsyncRelayCommand ReLoadCommand { get; protected set; }
        public RelayCommand CopyCommand { get; protected set; }
        public AsyncRelayCommand OpenCommand { get; protected set; }
        public RelayCommand CopyExceptionCommand { get; protected set; }

        public event Action<ComicPageInfo<TResource>> SkipAtConcurrent;
        public event Action<ComicPageInfo<TResource>> LoadDone;
        public event Action<ComicPageInfo<TResource>, Exception> LoadException;
        private void Init()
        {
            LoadCommand = new AsyncRelayCommand(LoadAsync);
            ReLoadCommand = new AsyncRelayCommand(ReloadAsync);
            CopyCommand = new RelayCommand(Copy);
            CopyExceptionCommand = new RelayCommand(CopyException);
            OpenCommand = new AsyncRelayCommand(OpenAsync);
        }
        public Task ReloadAsync()
        {
            Interlocked.Exchange(ref locker, SharedObject);
            return LoadAsync();
        }
        private IPlatformService PlatformService => AppEngine.GetRequiredService<IPlatformService>();
        public Task OpenAsync()
        {
            if (VisitPage is null)
            {
                return TaskHelper.GetComplatedTask();
            }
            return PlatformService.OpenAddressAsync(VisitPage.Page.TargetUrl);
        }
        public void Copy()
        {
            if (VisitPage != null)
            {
                PlatformService.Copy(VisitPage.Page.TargetUrl);
            }
        }
        public void CopyException()
        {
            if (Exception != null)
            {
                PlatformService.Copy(Exception.ToString());
            }
        }

        public void Release()
        {
            if (VisitPage != null && Interlocked.CompareExchange(ref locker, locker, null) != null)
            {
                HasException = false;
                LoadSucceed = false;
                EmitLoad = false;
                var vp = VisitPage;
                VisitPage = null;
                task = null;
                if (vp.Resource is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        public async Task LoadAsync()
        {
            Debug.Assert(PageSlots != null);
            try
            {
                if (Interlocked.CompareExchange(ref locker, null, locker) != null)
                {
                    HasException = false;
                    LoadSucceed = false;
                    Loading = true;
                    try
                    {
                        task = PageSlots.GetAsync(Index);
                        VisitPage = await task;
                        Resource = VisitPage.Resource;
                        LoadSucceed = true;
                    }
                    catch (Exception ex)
                    {
                        Exception = ex;
                        task = null;
                        HasException = true;
                        Interlocked.CompareExchange(ref locker, SharedObject, null);
                        LoadException?.Invoke(this, ex);
                    }
                    finally
                    {
                        Loading = false;
                    }
                }
                else
                {
                    await task;
                    SkipAtConcurrent?.Invoke(this);
                }
            }
            finally
            {
                EmitLoad = true;
            }
        }
    }
}
