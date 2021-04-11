using System;
using System.Collections.Generic;

namespace Anf.Easy.Downloading
{
    public class RandomDownloadManager : AsyncDownloadManager
    {
        private readonly Random random=new Random();
        protected override DownloadTask GetDownloadTask()
        {
            if (this.Count==0)
            {
                return null;
            }
            return this[random.Next(0, Count)];
        }
        protected override void OnComplated(DownloadTask task)
        {
            Remove(task);
        }
    }
}
