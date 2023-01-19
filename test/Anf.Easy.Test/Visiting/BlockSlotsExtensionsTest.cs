using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class BlockSlotsExtensionsTest
    {
        [TestMethod]
        public async Task GivenNullValueToCreateMap_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => BlockSlotsExtensions.GetCreatedValues<object>(null).ToArray());
            Assert.ThrowsException<ArgumentNullException>(() => BlockSlotsExtensions.GetCreatedValueMap<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => BlockSlotsExtensions.IsInRange<object>(null, 1));
            Assert.ThrowsException<ArgumentNullException>(() => BlockSlotsExtensions.ToDataCursor<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => BlockSlotsExtensions.ToLoadEnumerable<object>(null).ToArray());
            Assert.ThrowsException<ArgumentNullException>(() => BlockSlotsExtensions.ToLoadEnumerable<object>(null, 0).ToArray());
            Assert.ThrowsException<ArgumentNullException>(() => BlockSlotsExtensions.ToLoadEnumerable<object>(null, 0, 0).ToArray());
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => BlockSlotsExtensions.GetRangeAsync<object>(null, 0, 0));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => BlockSlotsExtensions.GetAllAsync<object>(null));
        }
        [TestMethod]
        public async Task GotValueMap_ReturnMustDataToMap()
        {
            var slot = CreateSlot(10);
            var map = BlockSlotsExtensions.GetCreatedValueMap(slot);
            Assert.AreEqual(0, map.Count);
            for (int i = 0; i < slot.Size; i++)
            {
                await slot.GetAsync(i);
            }
            map = BlockSlotsExtensions.GetCreatedValueMap(slot);
            for (int i = 0; i < slot.Size; i++)
            {
                Assert.IsTrue(map.ContainsKey(i), i.ToString());
                Assert.IsNotNull(map[i]);
            }
        }
        [TestMethod]
        public async Task GotCreatedValues_ReturnMustCreatedValues()
        {
            var slot = CreateSlot(10);
            var val = BlockSlotsExtensions.GetCreatedValues(slot);
            Assert.AreEqual(0, val.Count());
            await slot.GetAsync(0);
            await slot.GetAsync(1);
            await slot.GetAsync(2);
            val = BlockSlotsExtensions.GetCreatedValues(slot);
            Assert.AreEqual(3, val.Count());
        }
        private NullBlockSlots CreateSlot(int count)
        {
            var slot = new NullBlockSlots(count);
            for (int i = 0; i < slot.Datas.Length; i++)
            {
                slot.Datas[i] = () => new object();
            }
            return slot;
        }
        [TestMethod]
        [DataRow(0, null)]
        [DataRow(1, 3)]
        [DataRow(4, 4)]
        [DataRow(5, null)]
        public async Task GotLoadEnumerable_ReturnLoadEnumerable(int start, int? end)
        {
            var slot = CreateSlot(10);
            var enu = BlockSlotsExtensions.ToLoadEnumerable(slot, start, end).ToArray();
            var count = Math.Max(0, (end ?? slot.Size) - start);
            Assert.AreEqual(count, enu.Length);
            foreach (var item in enu)
            {
                await item();
            }
            Assert.AreEqual(count, slot.GetCreatedValues().Count());
        }
        [TestMethod]
        [DataRow(-1, null)]
        [DataRow(99, 3)]
        [DataRow(4, -1)]
        [DataRow(0, -9)]
        [DataRow(0, 99)]
        [DataRow(88, null)]
        [DataRow(int.MinValue, int.MaxValue)]
        public void GotLoadEnumerableWithOutOfRange_MustThrowException(int start, int? end)
        {
            var slot = CreateSlot(10);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => BlockSlotsExtensions.ToLoadEnumerable(slot, start, end).ToArray());
        }
        [TestMethod]
        public async Task ToDataCursor_MustReturnACursor()
        {
            var slot = CreateSlot(10);
            var cur = BlockSlotsExtensions.ToDataCursor(slot);
            Assert.AreEqual(slot.Size, cur.Count);
            var datas = new object[slot.Size];
            for (int i = 0; i < cur.Count; i++)
            {
                 await cur.MoveAsync(i);
                datas[i] = cur.Current;
            }
            for (int i = 0; i < cur.Count; i++)
            {
                Assert.AreEqual(slot[i], datas[i], i.ToString());
            }
        }
        [TestMethod]
        [DataRow(10, 0)]
        [DataRow(10, 2)]
        [DataRow(10, 5)]
        [DataRow(10, 9)]
        [DataRow(1, 0)]
        [DataRow(100, 0)]
        public void GivenInRangeIndex_MustReturnTrue(int size, int index)
        {
            var slot = CreateSlot(size);
            var inrange = BlockSlotsExtensions.IsInRange(slot, index);
            Assert.IsTrue(inrange);
        }
        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 2)]
        [DataRow(100, -1)]
        [DataRow(0, -1)]
        [DataRow(10, 999)]
        [DataRow(10, int.MaxValue)]
        [DataRow(10, int.MinValue)]
        public void GivenNotInRangeIndex_MustReturnTrue(int size, int index)
        {
            var slot = CreateSlot(size);
            var inrange = BlockSlotsExtensions.IsInRange(slot, index);
            Assert.IsFalse(inrange);
        }
        [TestMethod]
        [DataRow(1, 0, 0)]
        [DataRow(10, 2, 5)]
        [DataRow(10, 0, 10)]
        [DataRow(10, 9, 9)]
        [DataRow(10, 10, 10)]
        public async Task GivenNotInRangeIndex_MustReturnTrue(int size,int left,int right)
        {
            var slot = CreateSlot(size);
            var datas=await BlockSlotsExtensions.GetRangeAsync(slot, left, right);
            Assert.AreEqual(right - left, datas.Length);
            for (int i = 0; i < slot.Size; i++)
            {
                var val = slot[i];
                if (i >= left && i < right)
                {
                    Assert.IsNull(val);
                }
                else
                {
                    Assert.AreEqual(val, datas[i - left]);
                }
            }
        }
        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, -1, 0)]
        [DataRow(0, 0, -1)]
        [DataRow(0, -1, -1)]
        [DataRow(10, 1, 99)]
        [DataRow(10, 5, 1)]
        [DataRow(10, 77, 1)]
        [DataRow(10, 5, int.MaxValue)]
        public async Task GivenOutOfRange_MustThrowException(int size, int left, int right)
        {
            var slot = CreateSlot(size);
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() =>BlockSlotsExtensions.GetRangeAsync(slot, left, right));
        }
        [TestMethod]
        public async Task GetAll_MustReturnAllValue()
        {
            var slot = CreateSlot(10);
            var data = await BlockSlotsExtensions.GetAllAsync(slot);
            Assert.AreEqual(10, data.Length);
        }
    }
}
