using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Anf.Test
{
    [TestClass]
    public class SilentObservableCollectionTest
    {
        [TestMethod]
        public void AddRange_AllValueMustAdded()
        {
            var set = new HashSet<int>
            {
                1,2,3,4,5,67,8,91,9,87,6,5342,2,56,43,47
            };
            var coll = new SilentObservableCollection<int>();
            coll.AddRange(set);
            Assert.AreEqual(set.Count, coll.Count);
            var inSetCount = coll.Distinct().Where(x => set.Contains(x)).Count();
            Assert.AreEqual(set.Count, inSetCount);
        }
        [TestMethod]
        public void AddRange_EventMustBeFired()
        {
            var coll=new SilentObservableCollection<int>();
            NotifyCollectionChangedEventArgs arg = null;
            coll.CollectionChanged += (_, e) => arg = e;
            coll.AddRange(1, 2, 3, 45);
            Assert.IsNotNull(arg);
        }
    }
}
