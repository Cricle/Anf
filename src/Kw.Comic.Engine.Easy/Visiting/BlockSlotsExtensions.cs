using System;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public static class BlockSlotsExtensions
    {
        public static IDataCursor<T> ToDataCursor<T>(this BlockSlots<T> blockSlots)
            where T : class
        {
            if (blockSlots is null)
            {
                throw new ArgumentNullException(nameof(blockSlots));
            }

            return new BlockSlotsDataCursor<T>(blockSlots);
        }
        public static bool IsInRange<T>(this BlockSlots<T> blockSlots,int index)
            where T:class
        {
            return index >= 0 && blockSlots.Size != 0 && index < blockSlots.Size;
        }
        public static Task<T[]> GetAllAsync<T>(this BlockSlots<T> blockSlots)
            where T : class
        {
            return GetRangeAsync<T>(blockSlots, 0, blockSlots.Size);
        }
        public static async Task<T[]> GetRangeAsync<T>(this BlockSlots<T> blockSlots, int from, int? to)
            where T : class
        {
            if (blockSlots is null)
            {
                throw new ArgumentNullException(nameof(blockSlots));
            }
            if (to>=blockSlots.Size)
            {
                throw new ArgumentOutOfRangeException($"to must less {blockSlots.Size}");
            }
            var t = to ?? blockSlots.Size;
            if (from >= t)
            {
#if NET461 || NETSTANDARD2_0
                return Array.Empty<T>();
#else
                return new T[0];
#endif
            }
            var rets = new T[t - from];
            for (int i = from; i < t; i++)
            {
                rets[i] = await blockSlots.GetAsync(i);
            }
            return rets;
        }
    }
}
