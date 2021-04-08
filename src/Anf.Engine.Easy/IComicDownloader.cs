using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy
{
    public interface IComicDownloader
    {
        Func<Task>[] EmitTasks(ComicDownloadRequest request, CancellationToken token = default);
    }
}
