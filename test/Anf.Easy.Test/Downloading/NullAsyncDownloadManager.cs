using Anf.Easy.Downloading;
using System;
using System.Linq;

namespace Anf.Easy.Test.Downloading
{
    internal class NullAsyncDownloadManager : AsyncDownloadManager
    {
        public bool IsOnComplated { get; set; }
        public bool IsOnException { get; set; }
        public bool IsOnStop { get; set; }
        public bool IsOnStart { get; set; }

        protected override DownloadTask GetDownloadTask()
        {
            return this.FirstOrDefault();
        }
        protected override void OnComplated(DownloadTask task)
        {
            IsOnComplated = true;
        }
        protected override void OnException(DownloadTask task, Exception exception)
        {
            IsOnException = true;
        }
        protected override void OnStop()
        {
            IsOnStop = true;
        }
        protected override void OnStart()
        {
            IsOnStart = true;
        }
    }
}
