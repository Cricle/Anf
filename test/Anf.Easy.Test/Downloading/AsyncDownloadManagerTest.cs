using Anf.Easy.Downloading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Downloading
{
    [TestClass]
    public class AsyncDownloadManagerTest
    {
        [TestMethod]
        public void CallStart_IsStartIsTrueAndTaskWasCreated_StopIsReversal()
        {
            var mgr = new NullAsyncDownloadManager();
            Assert.IsNotNull(mgr.SyncRoot);
            Assert.IsFalse(mgr.IsStart);
            mgr.Start();
            Assert.IsTrue(mgr.IsStart);
            Assert.IsNotNull(mgr.Task);
            Assert.AreNotEqual(TaskStatus.WaitingForActivation, mgr.Task.Status);
            mgr.Stop();
            Assert.IsFalse(mgr.IsStart);
            Assert.IsTrue(mgr.CancellationToken.IsCancellationRequested);
        }
        [TestMethod]
        public async Task Start_Stop_Start_CancellationTokenMustBeReInit()
        {
            var mgr = new NullAsyncDownloadManager();
            mgr.Start();
            await Task.Delay(1000);
            mgr.Stop();
            mgr.Start();
            Assert.IsFalse(mgr.CancellationToken.IsCancellationRequested);
            mgr.Stop();
        }
        [TestMethod]
        public async Task WhenStart_Stop_Exception_Complated_CallbackWasRaised()
        {
            var mgr = new NullAsyncDownloadManager();
            mgr.Start();
            Assert.IsTrue(mgr.IsOnStart);
            mgr.Stop();
            Assert.IsTrue(mgr.IsOnStop);
            Func<Task> setTask = () => throw new Exception();
            mgr.Add(new DownloadTask(new Func<Task>[] { setTask }));
            mgr.Start();
            await Task.Delay(500);
            mgr.Stop();
            await mgr.Task;
            Assert.IsTrue(mgr.IsOnException);
            Assert.IsTrue(mgr.IsOnComplated);
        }
        [TestMethod]
        public async Task GivenTask_Run_TaskMustBeRun()
        {
            var mgr = new NullAsyncDownloadManager();
            var val = -1;
            Func<Task> setTask = () => Task.FromResult(val = 1);
            mgr.Add(new DownloadTask(new Func<Task>[] { setTask }));
            mgr.Start();
            await Task.Delay(100);
            mgr.Stop();
            await mgr.Task;
            Assert.AreEqual(1, val);
        }
    }
}
