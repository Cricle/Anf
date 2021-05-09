using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Test
{
    [TestClass]
    public class TaskHelperTest
    {
        [TestMethod]
        public async Task AwaitComplatedTask_MustBeOk()
        {
            var tk = TaskHelper.GetComplatedTask();
            await tk;
            Assert.IsTrue(tk.IsCompleted);
        }
    }
}
