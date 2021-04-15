using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    [TestClass]
    public class DownloadListenerBaseTest
    {
        class NullDownloadListener: DownloadListenerBase
        {

        }
        [TestMethod]
        public async Task CallImplement_MustNothingToDo()
        {
            var lister = new NullDownloadListener();
            await lister.BeginFetchPageAsync(null);
            await lister.CanceledAsync(null);
            await lister.ComplatedSaveAsync(null);
            await lister.EndFetchPageAsync(null);
            await lister.FetchPageExceptionAsync(null);
            await lister.NotNeedToSaveAsync(null);
            await lister.ReadyFetchAsync(null);
            await lister.ReadySaveAsync(null);
        }
    }
}
