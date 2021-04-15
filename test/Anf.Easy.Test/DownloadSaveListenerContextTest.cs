using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    [TestClass]
    public class DownloadSaveListenerContextTest
    {
        [TestMethod]
        public void GivenStream_CanCopyStream()
        {
            var mem = new MemoryStream();
            var ctx = new DownloadSaveListenerContext(null,null,null,default, mem);
            Assert.IsTrue(ctx.CanCopyStream);
        }
        private bool BytesSame(byte[] a,byte[] b)
        {
            if (a.Length!=b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i]!=b[i])
                {
                    return false;
                }
            }
            return true;
        }
        [TestMethod]
        public async Task GivenStream_CopyIt_ReturnStreamMustHasSameData()
        {
            var rand = new Random();
            var buffer = Enumerable.Range(0, 100)
                .Select(x=>(byte)rand.Next(0,byte.MaxValue))
                .ToArray();
            var mem = new MemoryStream();
            mem.Write(buffer, 0, buffer.Length);
            var ctx = new DownloadSaveListenerContext(null, null, null, default, mem);

            var copied = ctx.CopyStream();
            Assert.AreEqual(mem.Length, copied.Length);
            var buffer1 = new byte[buffer.Length];
            var copiedData1 = copied.Read(buffer1,0,buffer1.Length);
            Assert.IsTrue(BytesSame(buffer, buffer1));

            var copied2 = await ctx.CopyStreamAsync();
            Assert.AreEqual(mem.Length, copied2.Length);
            var buffer2 = new byte[buffer.Length];
            var copiedData2 = copied2.Read(buffer2, 0, buffer2.Length);
            Assert.IsTrue(BytesSame(buffer, buffer2));
        }
    }
}
