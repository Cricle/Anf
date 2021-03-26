using System;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    internal class BlockSlotsDataCursor<TValue> : DataCursorBase<TValue>
        where TValue:class
    {
        private readonly BlockSlots<TValue> blockSlots;

        public BlockSlotsDataCursor(BlockSlots<TValue> blockSlots)
        {
            this.blockSlots = blockSlots ?? throw new ArgumentNullException(nameof(blockSlots));
        }

        public override int Count => blockSlots.Size;

        protected override Task<TValue> LoadAsync(int index)
        {
            return blockSlots.GetAsync(index);
        }
    }
}
