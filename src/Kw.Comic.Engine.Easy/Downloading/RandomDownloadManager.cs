using System;
using System.Collections.Generic;

namespace Kw.Comic.Engine.Easy.Downloading
{
    public class RandomDownloadManager : AsyncDownloadManager
    {
        private readonly Random random=new Random();
        protected override DownloadTask GetDownloadTask()
        {
            return this[random.Next(0, Count)];
        }
        protected override void OnComplated(DownloadTask task)
        {
            Remove(task);
        }
    }
}
