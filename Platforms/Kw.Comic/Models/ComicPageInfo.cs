using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine.Easy.Visiting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Models
{
    public enum ComicPageInfoTypes
    {
        FromLoad,
        FromValue
    }
    public class ComicPageInfo<TResource> : ObservableObject
    {
        private IComicVisitPage<TResource> visitPage;
        private bool loading;
        private Exception exception;
        private TResource resource;

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
        private object locker = new object();
        private Task<IComicVisitPage<TResource>> task;

        public ComicPageInfo(PageSlots<TResource> pageSlots, int index)
        {
            PageSlots = pageSlots ?? throw new ArgumentNullException(nameof(pageSlots));
            Index = index;
            LoadCommand = new RelayCommand(() => _ = LoadAsync());
            PageInfoType = ComicPageInfoTypes.FromLoad;
        }
        public ComicPageInfo(IComicVisitPage<TResource> visitPage)
        {
            VisitPage = visitPage;
            PageInfoType = ComicPageInfoTypes.FromValue;
        }

        public ComicPageInfoTypes PageInfoType { get; }

        public PageSlots<TResource> PageSlots { get; }

        public int Index { get; }

        public RelayCommand LoadCommand { get; }

        public async Task LoadAsync()
        {
            if (PageSlots is null)
            {
                return;
            }
            if (Interlocked.CompareExchange(ref locker, null, locker) != null)
            {
                Loading = true;
                try
                {
                    task = PageSlots.GetAsync(Index);
                    VisitPage = await task;
                    Resource = VisitPage.Resource;
                }
                catch (Exception ex)
                {
                    Exception = ex;
                    task = null;
                    Interlocked.CompareExchange(ref locker, new object(), null);
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
