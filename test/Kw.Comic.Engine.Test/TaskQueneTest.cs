using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Test
{
    [TestClass]
    public class TaskQueneTest
    {
        private static Func<Task> TaskGen(Action does)
        {
            return () => Task.Run(does);
        }

        [TestMethod]
        public async Task AddSomeTaskFunc_RunIt_AllDone()
        {
            var tasks = Enumerable.Range(0, 10)
                .Select(x => TaskGen(() => { }))
                .ToArray();
            var res=await TaskQuene.RunVoidAsync(tasks);
            Assert.AreEqual(res.Length, tasks.Length);
            foreach (var item in res)
            {
                Assert.IsTrue(item.IsCompleted);
            }
        }
    }
}
