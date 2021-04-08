using System;
using System.Threading;

namespace Anf.Easy.Downloading
{
    public class DownloadBox
    {
        public DownloadBox(DownloadTask task, DownloadLink link)
        {
            Task = task;
            Link = link;
        }
        internal CancellationTokenSource TokenSource { get; set; }

        public event Action<DownloadBox> Canceled;

        public string Address => Link.Request.Entity.ComicUrl;

        public DownloadTask Task { get; }

        public DownloadLink Link { get; }

        public void Cancel()
        {
            if (TokenSource != null && !TokenSource.IsCancellationRequested)
            {
                TokenSource.Cancel();
                Canceled?.Invoke(this);
            }
        }
    }
}
