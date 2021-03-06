using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kw.Comic.Services
{
    public static class RedisKeyGenerator
    {
        /// <summary>
        /// 分布式锁获取超时时间
        /// </summary>
        public static readonly TimeSpan RedKeyOutTime = TimeSpan.FromSeconds(10);
        /// <summary>
        /// 输入错误缓存时间
        /// </summary>
        public static readonly TimeSpan InputErrorCacheTime = TimeSpan.FromMilliseconds(600);
        /// <summary>
        /// 平常缓存的时间
        /// </summary>
        public static readonly TimeSpan NormalCacheTime = TimeSpan.FromSeconds(1);
        /// <summary>
        /// 不经常变的缓存时间基数
        /// </summary>
        public static readonly TimeSpan InfrequentCacheTime = TimeSpan.FromSeconds(5);

        public const string DefaultSplit = "_";

        public const string NullString = "(Null)";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Concat(string header,params object[] parts)
        {
            return ConcatWithSplit(header, DefaultSplit, parts);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatWithSplit(string header,string split,params object[] parts)
        {
            if (parts is null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            if (parts.Length == 0)
            {
                return header;
            }
            if (parts.Length == 1)
            {
                return header + split + parts[0] ?? NullString;
            }
            var objs=new object[parts.Length];
            parts.CopyTo(objs, 0);
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] == null)
                {
                    objs[i] = NullString;
                }
            }
            return string.Concat(header + split, string.Join(split, objs));
        }
    }
}
