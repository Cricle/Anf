using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Anf.Easy.Visiting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public bool HasException
        {
            get { return hasException; }
            private set => Set(ref hasException, value);
        }

        public bool LoadSucceed
        {
            get { return loadSucceed; }
            private set => Set(ref loadSucceed, value);
        }

        public TResource Resource
        {
            get { return resource; }
            private set => Set(ref resource, value);
        }

        public Exception Exception
        {
            get { return exception; }
            private set => Set(ref exception, value);
        }

        public bool Loading
        {
            get { return loading; }
            private set => Set(ref loading, value);
        }

        public IComicVisitPage<TResource> VisitPage
        {
            get => visitPage;
            private set => Set(ref visitPage, value);
        }
        private object locker = SharedObject;
        private Task<IComicVisitPage<TResource>> task;

        public ComicPageInfo(PageSlots<TResource> pageSlots, int index)
        {
            PageSlots = pageSlots ?? throw new ArgumentNullException(nameof(pageSlots));
            Index = index;
            LoadCommand = new RelayCommand(() => _ = LoadAsync());
            ReLoadCommand = new RelayCommand(() => _ = ReloadAsync());
            PageInfoType = ComicPageInfoTypes.FromLoad;
        }
        public ComicPageInfo(IComicVisitPage<TResource> visitPage)
        {
            VisitPage = visitPage;
            PageInfoType = ComicPageInfoTypes.FromValue;
            LoadCommand = new RelayCommand(() => _ = LoadAsync());
            ReLoadCommand = new RelayCommand(() => _ = ReloadAsync());
        }

        public ComicPageInfoTypes PageInfoType { get; }

        public PageSlots<TResource> PageSlots { get; }

        public int Index { get; }

        public RelayCommand LoadCommand { get; }
        public RelayCommand ReLoadCommand { get; }
        public Task ReloadAsync()
        {
            Interlocked.Exchange(ref locker, SharedObject);
            return LoadAsync();
        }
        public async Task LoadAsync()
        {
            if (PageSlots is null)
            {
                return;
            }
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
                }
                finally
                {
                    Loading = false;
                }
            }
            else
            {
                await task;
            }
        }
    }
}
