using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Kw.Comic.Engine.Test
{
    [TestClass]
    public class ObservableCollectionExtensionTest
    {
        [TestMethod]
        public void GivenNotSortArray_SortIt_MustSorted()
        {
            var obser = new ObservableCollection<int>
            {
               -123 ,2312,3,13,211,4213,5,67,8,6,54,34,21,251,12,4,23,908786435,42,-23145,0,-12
            };
            obser.Sort(x => x);
            for (int i = 1; i < obser.Count; i++)
            {
                if (obser[i - 1] > obser[i])
                {
                    Assert.Fail();
                }
            }
            obser.SortDescending(x => x);
            for (int i = 1; i < obser.Count; i++)
            {
                if (obser[i - 1] < obser[i])
                {
                    Assert.Fail();
                }
            }
        }
        [TestMethod]
        public void GivenEmptyArray_SortIt_NothingTodo()
        {
            var val = new ObservableCollection<int>();
            val.Sort(x => x);
            Assert.AreEqual(0, val.Count);
            val.SortDescending(x => x);
            Assert.AreEqual(0, val.Count);
        }
    }
}
