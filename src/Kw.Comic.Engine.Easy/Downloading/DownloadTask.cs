using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Downloading
{
    /// <summary>
    /// 下载任务
    /// </summary>
    public class DownloadTask
    {
        private int position;

        private readonly Func<Task>[] tasks;

        public DownloadTask(Func<Task>[] tasks, CancellationToken cancellationToken = default)
        {
            this.tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
            CancellationToken = cancellationToken;
            Max = tasks.Length;
        }

        public int Position => position;

        public int Max { get; }

        public bool IsDone => position >= Max;

        public IReadOnlyList<Func<Task>> Tasks => tasks;

        public CancellationToken CancellationToken { get; }

        public event Action<DownloadTask, int> MovedNext;
        public event Action<DownloadTask, int> Seeked;
        public event Action<DownloadTask> Done;

        public void Seek(int pos)
        {
            if (pos >= Max || pos < 0)
            {
                throw new ArgumentOutOfRangeException(pos.ToString());
            }
            var r = Interlocked.Exchange(ref position, pos);
            Seeked?.Invoke(this, r);
        }

        public async Task<bool?> MoveNextAsync()
        {
            if (CancellationToken.IsCancellationRequested)
            {
                return false;
            }
            if (position >= Max)
            {
                return null;
            }
            var pos = Interlocked.Increment(ref position);
            if (pos < tasks.Length)
            {
                var tsk = tasks[pos]();
                await tsk;
                MovedNext?.Invoke(this, pos);
                return true;
            }
            else
            {
                Done?.Invoke(this);
            }
            return false;
        }
    }
}
