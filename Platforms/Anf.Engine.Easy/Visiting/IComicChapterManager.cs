using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    public interface IComicChapterManager<TResource>
    {
        ChapterWithPage ChapterWithPage { get; }

        Task<IComicVisitPage<TResource>> GetVisitPageAsync(int index);
    }
}
