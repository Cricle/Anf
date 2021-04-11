using Anf.Easy.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Store
{
    [TestClass]
    public class Md5StoreSeviceTest : FileStoreSeviceTestBase
    {
        public Md5StoreSeviceTest()
            : base(()=>FileStoreService.FromMd5Default(AppDomain.CurrentDomain.BaseDirectory))
        {
        }
    }
}
