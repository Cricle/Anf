using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public static class DataCursorExtensions
    {
        public static bool IsInRange<T>(this IDataCursor<T> cursor, int index)
        {
            if (cursor is null)
            {
                throw new ArgumentNullException(nameof(cursor));
            }

            if (cursor.Count == 0)
            {
                return false;
            }
            return index >= 0 && index < cursor.Count;
        }
        public static bool IsEnd<T>(this IDataCursor<T> cursor)
        {
            return cursor.Count == 0 || cursor.CurrentIndex == cursor.Count - 1;
        }
        public static bool IsFirst<T>(this IDataCursor<T> cursor)
        {
            return cursor.Count == 0;
        }
        public static int RightCount<T>(this IDataCursor<T> cursor)
        {
            return cursor.Count - cursor.CurrentIndex - 1;
        }
        public static Task<bool> MoveFirstAsync<T>(this IDataCursor<T> cursor)
        {
            if (cursor is null)
            {
                throw new ArgumentNullException(nameof(cursor));
            }

            return cursor.MoveAsync(0);
        }
        public static Task<bool> MoveLastAsync<T>(this IDataCursor<T> cursor)
        {
            if (cursor is null)
            {
                throw new ArgumentNullException(nameof(cursor));
            }

            return cursor.MoveAsync(cursor.Count - 1);
        }
        public static Task<bool> MoveNextAsync<T>(this IDataCursor<T> cursor)
        {
            if (cursor is null)
            {
                throw new ArgumentNullException(nameof(cursor));
            }

            return cursor.MoveAsync(cursor.CurrentIndex + 1);
        }
        public static Task<bool> MovePrevAsync<T>(this IDataCursor<T> cursor)
        {
            if (cursor is null)
            {
                throw new ArgumentNullException(nameof(cursor));
            }

            return cursor.MoveAsync(cursor.CurrentIndex - 1);
        }
    }
}
