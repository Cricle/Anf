using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;

namespace StackExchange.Redis
{
    internal static class RedisValueExtensions
    {
        private static readonly ConcurrentDictionary<Type, object> emptyStructs = new ConcurrentDictionary<Type, object>();
#if NET5_0_OR_GREATER
        public static async IAsyncEnumerable<RedisKey[]> SetScanAsync(this IDatabase db, string patter, int pageSize)
        {
            var count = 0L;
            do
            {
                var res = await db.ExecuteAsync("scan", count, "match", patter, "count", pageSize);
                var f = ((RedisResult[])res);
                count = ((long)f[0]);
                yield return (((RedisKey[])f[1]));
            } while (count != 0);
        }
#endif
        public static object Get(this in RedisValue value, Type type)
        {
            if (!value.HasValue || value.IsNull)
            {
                if (type.IsClass)
                {
                    return null;
                }
                return emptyStructs.GetOrAdd(type, x => Activator.CreateInstance(x));
            }
            if (type.IsPrimitive)
            {
                if (type == typeof(long))
                {
                    if (value.TryParse(out long l))
                        return l;
                    else
                        return default;
                }
                else if (type == typeof(int))
                {
                    if (value.TryParse(out int l))
                        return l;
                    else
                        return default;
                }
                else if (type == typeof(double))
                {
                    if (value.TryParse(out double l))
                        return l;
                    else
                        return default;
                }
                else
                {
                    try
                    {
                        var val = value.ToString();
                        var v = Convert.ChangeType(val, type);
                        return v;
                    }
                    catch (Exception) { }
                }
            }
            else if (type == typeof(string))
            {
                return value.ToString();
            }
            else if (type.IsEnum)
            {
                if (TryParseEnum(type, value.ToString(), out var enu))
                {
                    return enu;
                }
            }
            else if (type.IsClass || type.IsValueType)
            {
                var val = value.ToString();
                var obj = JsonSerializer.Deserialize(val, type);
                return obj;
            }
            return default;
        }
        private static bool TryParseEnum(Type type, string val, out object res)
        {
#if NETSTANDARD2_0
            try
            {
                res = Enum.Parse(type, val);
                return true;
            }
            catch (Exception)
            {
                res = null;
                return false;
            }
#else
            return Enum.TryParse(type, val, out res);
#endif
        }
        public static T Get<T>(this in RedisValue value)
        {
            return (T)Get(value, typeof(T));
        }

        public static long GetLong(this RedisValue value)
        {
            return value.Get<long>();
        }
        public static double GetDouble(this RedisValue value)
        {
            return value.Get<double>();
        }
        public static int GetInt(this RedisValue value)
        {
            return value.Get<int>();
        }
    }
}
