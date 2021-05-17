using Anf.Easy.Downloading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Downloading
{
    [TestClass]
    public class DownloadManagersTest
    {
        private async Task<int> Run(AsyncDownloadManager mgr,int taskCount,int preCount)
        {
            var val = 0;
            for (int i = 0; i < taskCount; i++)
            {
                var tasks = Enumerable.Range(0, preCount).Select(x => new Func<Task>(() => Task.FromResult(Interlocked.Increment(ref val)))).ToArray();
                mgr.Add(new DownloadTask(tasks));
            }
            mgr.Start();
            var tks = new CancellationTokenSource();
            var timeOutTask = Task.Delay(TimeSpan.FromMinutes(30)).ContinueWith(_ => tks.Cancel());
            while (!tks.IsCancellationRequested)
            {
                if (mgr.Count == 0)
                {
                    break;
                }
                await Task.Delay(100);
            }
            mgr.Stop();
            await mgr.Task;
            return val;
        }
        [TestMethod]
        [DataRow(10, 10)]
        [DataRow(10, 0)]
        [DataRow(10, 1)]
        [DataRow(1, 1)]
        public async Task GivenSomeTasks_RandomRunIt_AllTaskMustBeRun(int taskCount, int preCount)
        {
            var mgr = new RandomDownloadManager();
            var val = await Run(mgr, taskCount, preCount);
            Assert.AreEqual(taskCount * preCount, val);
        }
        [TestMethod]
        [DataRow(10, 10)]
        [DataRow(10, 0)]
        [DataRow(10, 1)]
        [DataRow(1, 1)]
        public async Task GivenSomeTasks_QueneRunIt_AllTaskMustBeRun(int taskCount, int preCount)
        {
            var mgr = new QueneDownloadManager();
            var val = await Run(mgr, taskCount, preCount);
            Assert.AreEqual(taskCount * preCount, val);
        }
        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(10, 10)]
        [DataRow(10, 0)]
        [DataRow(10, 1)]
        [DataRow(1, 1)]
        public async Task GivenSomeTasks_QueneWithEndRunIt_AllTaskMustBeRun(int taskCount, int preCount)
        {
            var mgr = new QueneDownloadManager { PeekType = QuenePeekTypes.End };
            var val = await Run(mgr, taskCount, preCount);
            Assert.AreEqual(taskCount * preCount, val);
        }
    }
}
