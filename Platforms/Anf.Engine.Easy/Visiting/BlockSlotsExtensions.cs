using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Anf.Easy.Visiting
{
    public static class BlockSlotsExtensions
    {
        public static IReadOnlyDictionary<int, T> GetCreatedValueMap<T>(this BlockSlots<T> blockSlots)
            where T : class
        {
            if (blockSlots is null)
            {
                throw new ArgumentNullException(nameof(blockSlots));
            }

            var dic = new Dictionary<int, T>();
            for (int i = 0; i < blockSlots.Size; i++)
            {
                var s = blockSlots[i];
                if (!(s is null))
                {
                    dic.Add(i, s);
                }
            }
            return dic;
        }
        public static IEnumerable<T> GetCreatedValues<T>(this BlockSlots<T> blockSlots)
            where T : class
        {
            if (blockSlots is null)
            {
                throw new ArgumentNullException(nameof(blockSlots));
            }

            for (int i = 0; i < blockSlots.Size; i++)
            {
                var s = blockSlots[i];
                if (!(s is null))
                {
                    yield return s;
                }
            }
        }
        public static IEnumerable<Func<Task<T>>> ToLoadEnumerable<T>(this BlockSlots<T> blockSlots, int start = 0, int? end = null)
            where T : class
        {
            if (blockSlots is null)
            {
                throw new ArgumentNullException(nameof(blockSlots));
            }
            if (start < 0 || end > blockSlots.Size || end < 0 || start > blockSlots.Size || start > end || blockSlots.Size == 0)
            {
                throw new ArgumentOutOfRangeException($"Must [{0},{blockSlots.Size}]");
            }
            while (start < blockSlots.Size && (end == null || start < end))
            {
                var i = start;
                yield return () => blockSlots.GetAsync(i);
                start++;
            }
        }
        public static IDataCursor<T> ToDataCursor<T>(this BlockSlots<T> blockSlots)
            where T : class
        {
            if (blockSlots is null)
            {
                throw new ArgumentNullException(nameof(blockSlots));
            }

            return new BlockSlotsDataCursor<T>(blockSlots);
        }
        public static bool IsInRange<T>(this BlockSlots<T> blockSlots, int index)
            where T : class
        {
            if (blockSlots is null)
            {
                throw new ArgumentNullException(nameof(blockSlots));
            }

            return index >= 0 && blockSlots.Size != 0 && index < blockSlots.Size;
        }
        public static Task<T[]> GetAllAsync<T>(this BlockSlots<T> blockSlots)
            where T : class
        {
            if (blockSlots is null)
            {
                throw new ArgumentNullException(nameof(blockSlots));
            }

            return GetRangeAsync(blockSlots, 0, blockSlots.Size - 1);
        }
        public static async Task<T[]> GetRangeAsync<T>(this BlockSlots<T> blockSlots, int from, int? to)
            where T : class
        {
            if (blockSlots is null)
            {
                throw new ArgumentNullException(nameof(blockSlots));
            }
            if (from < 0 || to < 0 || to > blockSlots.Size || from > blockSlots.Size || from > to || blockSlots.Size == 0)
            {
                throw new ArgumentOutOfRangeException($"to must less {blockSlots.Size}");
            }
            var t = to ?? (blockSlots.Size - 1);
            if (from >= t)
            {
#if NET461 || NETSTANDARD2_0
                return Array.Empty<T>();
#else
                return new T[0];
#endif
            }
            var rets = new T[t - from + 1];
            for (int i = from; i <= t; i++)
            {
                rets[i] = await blockSlots.GetAsync(i);
            }
            return rets;
        }
    }
}
