using Kw.Comic.Rendering;
using Kw.Comic.Visit;
using Kw.Comic.Visit.Interceptors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.PreLoading
{
    public class ComicPageInfoCollection<TVisitor>
        where TVisitor : ChapterVisitorBase
    {
        public ComicPageInfoCollection()
        {
            PreLoading = 5;
            Directions = PreLoadingDirections.Right;
        }

        public IPageLoadInterceptor<TVisitor> Interceptor { get; set; }

        public PreLoadingDirections Directions { get; set; }

        public PreLoadingMode Mode { get; set; }

        public bool AsyncLoad { get; set; }

        public int? PreLoading { get; set; }

        public PageCursorBase<TVisitor> PageCursor { get; set; }

        public event Action<ComicPageInfoCollection<TVisitor>, int> ActivedDone;

        public async Task UnActiveAllAsync()
        {
            foreach (var item in PageCursor.Datas)
            {
                await item.UnLoadAsync();
            }
        }
        protected virtual Task OnActiveAsync(TVisitor pageInfo)
        {
            var inter = Interceptor;
            if (inter!=null)
            {
                return inter.LoadAsync(PageCursor, pageInfo);
            }
            return pageInfo.LoadAsync();
        }

        public async Task ActiveAsync(int index)
        {
            var cur = PageCursor;
            if (cur==null)
            {
                return;
            }
            if (index < 0 || index >= cur.Length)
            {
                return;
            }
            var left = index;
            var right = index;
            if (PreLoading == null)
            {
                left = 0;
                right = cur.Length;
            }
            else
            {
                if ((Directions & PreLoadingDirections.Left) != 0)
                {
                    left = Math.Max(0, index - PreLoading.Value);
                }
                if ((Directions & PreLoadingDirections.Right) != 0)
                {
                    right = Math.Min(index + PreLoading.Value, cur.Length);
                }
            }
            if (Mode == PreLoadingMode.UnLoadOnReplace)
            {
                for (int i = 0; i < left; i++)
                {
                    var val = cur[i];
                    if (val.IsLoaded)
                    {
                        _ = val.UnLoadAsync();
                    }
                }
                for (int i = right + 1; i < cur.Length; i++)
                {
                    var val = cur[i];
                    if (val.IsLoaded)
                    {
                        _ = val.UnLoadAsync();
                    }
                }
            }
            if (left == right)
            {
                await cur[left].LoadAsync();
            }
            else
            {
                if (AsyncLoad)
                {
                    var tasks = new List<Task>();
                    for (int i = left; i < right; i++)
                    {
                        tasks.Add(OnActiveAsync(cur[i]));
                    }
                    await Task.WhenAll(tasks);
                }
                else
                {
                    for (int i = left; i < right; i++)
                    {
                        await OnActiveAsync(cur[i]);
                    }
                }
            }
            ActivedDone?.Invoke(this, index);
        }
    }
}
