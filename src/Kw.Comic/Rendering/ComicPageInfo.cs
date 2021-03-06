using Kw.Comic.Visit;
using Kw.Comic.Visit.Interceptors;
using Kw.Core.Input;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Rendering
{
    public abstract class ComicPageInfo<TVisitor,TImage> : ViewModelBase, IDisposable
        where TVisitor : ChapterVisitorBase
    {
        public ComicPageInfo(TVisitor visitor,PageCursorBase<TVisitor> pageCursor)
        {
            PageCursor = pageCursor;
            Visitor = visitor;
        }

        private int locker;
        private bool loading;
        private TImage image;
        private bool error;
        private string errorMsg;
        private bool done;
        private IPageLoadInterceptor<TVisitor> interceptor;

        public IPageLoadInterceptor<TVisitor> Interceptor
        {
            get { return interceptor; }
            set => RaisePropertyChanged(ref interceptor, value);
        }

        public bool Done
        {
            get { return done; }
            set => RaisePropertyChanged(ref done, value);
        }

        public string ErrorMsg
        {
            get { return errorMsg; }
            private set => RaisePropertyChanged(ref errorMsg, value);
        }

        public bool Error
        {
            get { return error; }
            private set => RaisePropertyChanged(ref error, value);
        }

        public TImage Image
        {
            get { return image; }
            private set
            {
                RaisePropertyChanged(ref image, value);
            }
        }

        public bool Loading
        {
            get { return loading; }
            private set => RaisePropertyChanged(ref loading, value);
        }

        public PageCursorBase<TVisitor> PageCursor { get; }

        public TVisitor Visitor { get; }

        public void Dispose()
        {
            Visitor.Dispose();
            UnLoadAsync().GetAwaiter().GetResult();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Entry()
        {
            return Interlocked.CompareExchange(ref locker, 1, 0) == 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Exist()
        {
            return Interlocked.CompareExchange(ref locker, 0, 1) == 1;
        }
        public virtual async Task UnLoadAsync()
        {
            if (Image == null)
            {
                return;
            }
            Error = false;
            if (!Entry())
            {
                return;
            }
            try
            {
                if (Image == null)
                {
                    return;
                }
                Image = default;
                await Visitor.UnLoadAsync();
                Done = false;
            }
            catch (Exception ex)
            {
                Error = true;
                ErrorMsg = ex.Message;
            }
            finally
            {
                Exist();
            }
        }
        protected abstract Task<TImage> OnLoadResourceAsync(TVisitor visitor);
        public async Task LoadAsync()
        {
            if (Image != null)
            {
                return;
            }
            if (!Entry())
            {
                return;
            }
            try
            {
                if (Image != null)
                {
                    return;
                }
                Loading = true;
                if (!Visitor.IsLoaded)
                {
                    var inter = Interceptor;
                    if (inter != null)
                    {
                        await inter.LoadAsync(PageCursor, Visitor);
                    }
                    else
                    {

                        await Visitor.LoadAsync();
                    }
                }
                Image = await OnLoadResourceAsync(Visitor);
                Done = true;
            }
            catch (Exception ex)
            {
                Error = true;
                ErrorMsg = ex.Message;
            }
            finally
            {
                Loading = false;
                Exist();
            }
        }
    }
}
