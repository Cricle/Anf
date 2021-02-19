using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public static class DataCursorExtensions
    {
        public static Task<bool> SetFirstAsync<T>(this DataCursor<T> cursor)
        {
            return cursor.SetIndexAsync(0);
        }
        public static Task<bool> SetLastAsync<T>(this DataCursor<T> cursor)
        {
            return cursor.SetIndexAsync(cursor.Length - 1);
        }
        public static Task<bool> NextAsync<T>(this DataCursor<T> cursor)
        {
            return cursor.SetIndexAsync(cursor.Index + 1);
        }
        public static Task<bool> PrevAsync<T>(this DataCursor<T> cursor)
        {
            return cursor.SetIndexAsync(cursor.Index - 1);
        }
        public static async Task LoadAllAsync<T>(this DataCursor<T> cursor)
        {
            for (int i = 0; i < cursor.Length; i++)
            {
                await cursor.LoadIndexAsync(i);
            }
        }
    }
}
