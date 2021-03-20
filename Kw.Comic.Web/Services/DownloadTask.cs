using Kw.Comic.Engine.Easy;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KwC.Services
{
    public class DownloadTask : IDisposable
    {
        public DownloadTask(string address,DownloadLink link)
        {
            Link = link;
            TokenSource = new CancellationTokenSource();
            tasks = Link.Downloader.EmitTasks(Link.Request, TokenSource.Token);
            Max = tasks.Length;
            Address = address;
        }
        private readonly Func<Task>[] tasks;
        private int position;

        public string Address { get; }

        public int Position => Volatile.Read(ref position);

        public int Max { get; }

        public bool IsDone => Position >= Max;

        public DownloadLink Link { get; }

        public CancellationTokenSource TokenSource { get; }

        public event Action<DownloadTask,int> MovedNext;

        public async Task<bool> MoveNextAsync()
        {
            var pos = Interlocked.Increment(ref position);
            if (pos < tasks.Length)
            {
                var tsk = tasks[pos]();
                await tsk;
                MovedNext?.Invoke(this, pos);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            TokenSource.Cancel();
            TokenSource.Dispose();
        }
    }
}
