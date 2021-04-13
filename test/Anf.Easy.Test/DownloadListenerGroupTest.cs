using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    [TestClass]
    public class DownloadListenerGroupTest
    {
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(10)]
        public async Task GivenAnyListener_CallAllMethod_CollectionItemMethodsMustBeCalled(int size)
        {
            var group = new DownloadListenerGroup();
            for (int i = 0; i < size; i++)
            {
                group.Add(new NullDownloadListener());
            }
            await group.BeginFetchPageAsync(null);
            await group.CanceledAsync(null);
            await group.CanceledAsync(null);
            await group.ComplatedSaveAsync(null);
            await group.EndFetchPageAsync(null);
            await group.FetchPageExceptionAsync(null);
            await group.NotNeedToSaveAsync(null);
            await group.ReadyFetchAsync(null);
            await group.ReadySaveAsync(null);

            for (int i = 0; i < size; i++)
            {
                var val = (NullDownloadListener)group[i];
                Assert.IsTrue(val.IsBeginFetchPageAsync);
                Assert.IsTrue(val.IsCanceledAsync);
                Assert.IsTrue(val.IsCanceledAsync);
                Assert.IsTrue(val.IsComplatedSaveAsync);
                Assert.IsTrue(val.IsEndFetchPageAsync);
                Assert.IsTrue(val.IsNotNeedToSaveAsync);
                Assert.IsTrue(val.IsFetchPageExceptionAsync);
                Assert.IsTrue(val.IsReadyFetchAsync);
                Assert.IsTrue(val.IsReadySaveAsync);
            }
        }
    }
}
