using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Anf.Easy.Downloading
{
    public class QueneDownloadManager : AsyncDownloadManager
    {
        public QuenePeekTypes PeekType { get; set; }

        protected override DownloadTask GetDownloadTask()
        {
            if (Count != 0)
            {
                if (PeekType == QuenePeekTypes.End)
                {
                    return this[Count - 1];
                }
                return this[0];
            }
            return null;
        }
        protected override void OnComplated(DownloadTask task)
        {
            Remove(task);
        }
    }
}
