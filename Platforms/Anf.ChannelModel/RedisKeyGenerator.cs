using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Anf.ChannelModel.KeyGenerator
{
    public static class RedisKeyGenerator
    {
        /// <summary>
        /// 分布式锁获取超时时间
        /// </summary>
        public static readonly TimeSpan RedKeyOutTime = TimeSpan.FromSeconds(20);
        /// <summary>
        /// 输入错误缓存时间
        /// </summary>
        public static readonly TimeSpan InputErrorCacheTime = TimeSpan.FromMilliseconds(600);
        /// <summary>
        /// 平常缓存的时间
        /// </summary>
        public static readonly TimeSpan NormalCacheTime = TimeSpan.FromSeconds(15);
        /// <summary>
        /// 不经常变的缓存时间基数
        /// </summary>
        public static readonly TimeSpan InfrequentCacheTime = TimeSpan.FromSeconds(5);
        
        public static readonly TimeSpan LockerWaitTime = TimeSpan.FromSeconds(1);

        public const string DefaultSplit = "_";

        public const string NullString = "(Null)";
        public static string Concat(string header, params object[] parts)
        {
            return ConcatWithSplit(header, DefaultSplit, parts);
        }
        public static string Concat(string header, object part1)
        {
            return ConcatWithSplit(header, DefaultSplit, part1);
        }
        public static string Concat(string header, object part1,object part2)
        {
            return ConcatWithSplit(header, DefaultSplit, part1, part2);
        }
        public static string Concat(string header, object part1,object part2,object part3)
        {
            return ConcatWithSplit(header, DefaultSplit, part1,part2,part3);
        }
        public static string Concat(string header, object part1, object part2, object part3, object part4)
        {
            return ConcatWithSplit(header, DefaultSplit, part1, part2, part3, part4);
        }
        public static string ConcatWithSplit(string header, string split, object part1)
        {
            return string.Concat(header, split, part1 ?? NullString);
        }
        public static string ConcatWithSplit(string header, string split, object part1, object part2)
        {
            return string.Concat(header, split, part1 ?? NullString, split, part2 ?? NullString);
        }
        public static string ConcatWithSplit(string header, string split, object part1, object part2, object part3)
        {
            return string.Concat(header, split, part1 ?? NullString, split, part2 ?? NullString, split, part3 ?? NullString);
        }
        public static string ConcatWithSplit(string header, string split, object part1, object part2, object part3, object part4)
        {
            return string.Concat(header, split, part1 ?? NullString, split, part2 ?? NullString, split, part3 ?? NullString, split, part4 ?? NullString);
        }
        
        public static string ConcatWithSplit(string header, string split, params object[] parts)
        {
            if (parts is null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            var objs = new object[parts.Length];
            Array.Copy(parts, 0, objs, 0, parts.Length);
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] is null)
                {
                    objs[i] = NullString;
                }
            }
            return string.Concat(header, split, string.Join(split, objs));
        }
        private static readonly string OnceKey = "!Once.APS.Redis.KeyGenerator.Token";
        public static string CreateOnceToken(string typeName,object channel,object value)
        {
            return Concat(OnceKey, typeName, channel,value);
        }
        private static readonly Random random=new Random();
        public static TimeSpan GetEmitTime(TimeSpan time)
        {
            return time.Add(TimeSpan.FromMilliseconds(random.Next(10,100)));
        }
    }
}
