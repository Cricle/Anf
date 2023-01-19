using Anf.Easy.Downloading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Anf.Easy.Test.Downloading
{
    [TestClass]
    public class DownloadBoxTest
    {
        [TestMethod]
        public void GivenValueInit_PropertyValueMustEqualGiven()
        {
            var tsk = new DownloadTask(new Func<Task>[0]);
            var addr = "adsads";
            var link = new DownloadLink(null,null,new ComicDownloadRequest(null,new ComicEntity { ComicUrl=addr},null,null,null));
            var box = new DownloadBox(tsk, link);
            Assert.AreEqual(tsk, box.Task);
            Assert.AreEqual(link, box.Link);
            Assert.AreEqual(addr, box.Address);
        }
        [TestMethod]
        public void GivenCancelToken_CancelIt_MustFiredEventAndCanceled()
        {
            var box = new DownloadBox(null, default(DownloadLink));
            var tk = new CancellationTokenSource();
            box.TokenSource = tk;
            Assert.AreEqual(tk, box.TokenSource);
            object sender = null;
            box.Canceled += (o) =>
            {
                sender = o;
            };
            box.Cancel();
            Assert.IsTrue(tk.IsCancellationRequested);
            Assert.AreEqual(box, sender);
            sender = null;
            box.Cancel();
            Assert.IsNull(sender);
        }
    }
}
