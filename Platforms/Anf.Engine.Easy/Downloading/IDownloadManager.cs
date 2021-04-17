using System.Collections.Generic;

namespace Anf.Easy.Downloading
{
    public interface IOperatable
    {
        bool IsStart { get; }

        void Start();

        void Stop();
    }
    public interface IDownloadManager : IOperatable,IList<DownloadTask>
    {
    }
}
