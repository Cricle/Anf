using Kw.Comic.Rendering;
using Kw.Comic.Visit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Kw.Comic.PreLoading
{
    public class ComicPageInfoCollection<TPageInfo, TVisitor, TImage> : ObservableCollection<TPageInfo>
        where TVisitor : ChapterVisitorBase
        where TPageInfo: ComicPageInfo<TVisitor, TImage>
    {
        public ComicPageInfoCollection()
        {
            PreLoading = 5;
            Directions = PreLoadingDirections.Right;
        }

        public PreLoadingDirections Directions { get; set; }

        public PreLoadingMode Mode { get; set; }

        public bool AsyncLoad { get; set; }

        public int? PreLoading { get; set; }

        public event Action<ComicPageInfoCollection<TPageInfo, TVisitor, TImage>, int> ActivedDone;

        public async Task UnActiveAllAsync()
        {
            for (int i = 0; i < Count; i++)
            {
                await this[i].UnLoadAsync();
            }
        }

        public async Task ActiveAsync(int index)
        {
            if (index < 0 || index >= Count)
            {
                return;
            }
            var left = index;
            var right = index;
            if (PreLoading == null)
            {
                left = 0;
                right = Count;
            }
            else
            {
                if ((Directions & PreLoadingDirections.Left) != 0)
                {
                    left = Math.Max(0, index - PreLoading.Value);
                }
                if ((Directions & PreLoadingDirections.Right) != 0)
                {
                    right = Math.Min(index + PreLoading.Value, Count);
                }
            }
            if (Mode == PreLoadingMode.UnLoadOnReplace)
            {
                for (int i = 0; i < left; i++)
                {
                    var val = this[i];
                    if (val.Done)
                    {
                        _ = val.UnLoadAsync();
                    }
                }
                for (int i = right + 1; i < Count; i++)
                {
                    var val = this[i];
                    if (val.Done)
                    {
                        _ = val.UnLoadAsync();
                    }
                }
            }
            if (left == right)
            {
                await this[left].LoadAsync();
            }
            else
            {
                if (AsyncLoad)
                {
                    var tasks = new List<Task>();
                    for (int i = left; i < right; i++)
                    {
                        tasks.Add(this[i].LoadAsync());
                    }
                    await Task.WhenAll(tasks);
                }
                else
                {
                    for (int i = left; i < right; i++)
                    {
                        await this[i].LoadAsync();
                    }
                }
            }
            ActivedDone?.Invoke(this, index);
        }
    }
}
