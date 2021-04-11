using Anf.Easy.Downloading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Anf.Easy.Test.Downloading
{
    [TestClass]
    public class DownloadTaskTest
    {
        [TestMethod]
        public void GivenNullValueInit_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DownloadTask(null));
        }
        private DownloadTask CreateTask(int count,out Func<Task>[] values)
        {
            values = Enumerable.Range(0, count).Select(x => new Func<Task>(() => Task.FromResult(x))).ToArray();
            var task = new DownloadTask(values);
            return task;
        }
        [TestMethod]
        public void GivenValueInit_PropertyValueMustGiven()
        {
            var task = CreateTask(10,out var values);
            Assert.AreEqual(10, task.Max);
            Assert.IsFalse(task.IsDone);
            for (int i = 0; i < task.Tasks.Count; i++)
            {
                Assert.AreEqual(values[i], task.Tasks[i], i.ToString());
            }
            Assert.AreEqual(CancellationToken.None, task.CancellationToken);

            var tks = new CancellationTokenSource();
            task = new DownloadTask(values, tks.Token);
            Assert.AreEqual(tks.Token, task.CancellationToken);
        }
        [TestMethod]
        [DataRow(10, 0)]
        [DataRow(10, 2)]
        [DataRow(10, 5)]
        [DataRow(10, 9)]
        [DataRow(1, 0)]
        [DataRow(2, 1)]
        [DataRow(2, -1)]
        public void GivenValueInit_SeekIt_PositionMustOnSeek(int count, int index)
        {
            var task = CreateTask(count, out var values);
            task.Seek(index);
            Assert.AreEqual(index, task.Position);
        }
        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(2, 4)]
        [DataRow(5, 99)]
        [DataRow(10, int.MaxValue)]
        [DataRow(2, -10)]
        [DataRow(10, int.MinValue)]
        public void GivenValueInit_SeekOnNotExists_MustThrowException(int count, int index)
        {
            var task = CreateTask(count, out var values);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => task.Seek(index));
        }
        [TestMethod]
        public void GivenValueInit_SeekIt_SeekedEventMustBeFired()
        {
            var task = CreateTask(10, out var values);
            object sender = null;
            int idx = -1;
            task.Seeked += (o, e) =>
            {
                sender = o;
                idx = e;
            };
            task.Seek(4);
            Assert.AreEqual(task, sender);
            Assert.AreEqual(4, idx);
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(5)]
        [DataRow(10)]
        public async Task GivenValueInit_MoveNext(int size)
        {
            var task = CreateTask(size, out var values);
            for (int i = 0; i < size; i++)
            {
                var ok = await task.MoveNextAsync();
                Assert.AreEqual(i, task.Position);
                Assert.IsTrue(ok.Value);
            }
        }
        [TestMethod]
        public async Task GivenValueInit_MoveNext_SeekPrev_MoveNext_GotValueMustNull()
        {
            var task = CreateTask(10, out var values);
            var a=await task.MoveNextAsync();
            Assert.IsTrue(a.Value);
            task.Seek(9);
            await task.MoveNextAsync();
            var b = await task.MoveNextAsync();
            Assert.IsNull(b);
        }
        [TestMethod]
        public async Task GivenValueInit_MoveNext_MovedNextEventMustBeFired()
        {
            var task = CreateTask(10, out var values);
            object sender = null;
            int index = -1;
            task.MovedNext += (o, e) =>
            {
                sender = o;
                index = e;
            };
            var a = await task.MoveNextAsync();
            Assert.AreEqual(task, sender);
            Assert.AreEqual(0, index);
        }
        [TestMethod]
        [DataRow(0)]
        [DataRow(5)]
        public async Task GivenValueInit_MoveEnd_DoneEventMustBeFired(int size)
        {
            var task = CreateTask(size, out var values);
            object sender = null;
            task.Done += (o) =>
            {
                sender = o;
            };
            task.Seek(size - 1);
            await task.MoveNextAsync();
            Assert.AreEqual(task,sender);
            sender = null;
            await task.MoveNextAsync();
            Assert.IsNull(sender);
        }
        [TestMethod]
        public async Task GivenValueInit_CancelIt_MoveNext_MustNothingTodo()
        {
            var tks = new CancellationTokenSource();
            var task = new DownloadTask(new Func<Task>[] { () => Task.FromResult(0) }, tks.Token);
            tks.Cancel();
            var val = await task.MoveNextAsync();
            Assert.IsFalse(val.Value);
        }
    }
}
