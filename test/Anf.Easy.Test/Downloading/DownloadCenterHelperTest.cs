using Anf.Easy.Downloading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Downloading
{
    class NullDownloadManager : List<DownloadTask>, IDownloadManager
    {
        public bool IsStart { get; set; }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
    [TestClass]
    public class DownloadCenterHelperTest
    {
        [TestMethod]
        public void GivenNullCall_MustThrowException()
        {
            var mgr = new NullDownloadManager();
            var saver = new NullSaver();

            Assert.ThrowsException<ArgumentNullException>(() => DownloadCenterHelper.CreateDownloadCenter(null, mgr, saver));
            Assert.ThrowsException<ArgumentNullException>(() => DownloadCenterHelper.CreateQueneDownloadCenter(null, saver));
            Assert.ThrowsException<ArgumentNullException>(() => DownloadCenterHelper.CreateDownloadCenterFromService(null));
        }
        [TestMethod]
        public void GivenValueCreateDownloadCenter_MustReturnValue()
        {
            var mgr = new NullDownloadManager();
            var saver = new NullSaver();
            var provider = new ValueServiceProvider 
            {
                 ServiceMap=new Dictionary<Type, Func<object>>
                 {
                     [typeof(IComicSaver)] = () => saver,
                     [typeof(IDownloadManager)] = () => mgr,
                 }
            };

            var center = DownloadCenterHelper.CreateDownloadCenter(
                provider, mgr, saver);
            Assert.IsNotNull(center);

            center = DownloadCenterHelper.CreateQueneDownloadCenter(provider, saver);
            Assert.IsNotNull(center);

            center = DownloadCenterHelper.CreateDownloadCenterFromService(provider);
            Assert.IsNotNull(center);
        }
    }
}
