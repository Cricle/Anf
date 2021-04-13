using Anf.Easy.Visiting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class ValueComicChapterManager<T> : IComicChapterManager<T>
    {
        public ChapterWithPage ChapterWithPage { get; set; }

        public Dictionary<int,Func<IComicVisitPage<T>>> Map { get; set; }

        public Task<IComicVisitPage<T>> GetVisitPageAsync(int index)
        {
            return Task.FromResult<IComicVisitPage<T>>(Map[index]());
        }
    }
}
