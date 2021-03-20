using System.Collections.Generic;

namespace Kw.Comic.Engine.Easy.Downloading
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
