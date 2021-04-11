using Anf.Easy.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Anf.Easy.Test.Store
{
    [TestClass]
    public class DefaultStoreSeviceTest : FileStoreSeviceTestBase
    {
        public DefaultStoreSeviceTest()
            : base(()=>FileStoreService.FromDefault(AppDomain.CurrentDomain.BaseDirectory,MD5AddressToFileNameProvider.Instance))
        {
        }
    }
}
