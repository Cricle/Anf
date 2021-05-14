using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace StackExchange.Redis
{
    internal static class RedisValueExtensions
    {
        private static readonly ConcurrentDictionary<Type, object> emptyStructs = new ConcurrentDictionary<Type, object>();
        
        public static object Get(this RedisValue value,Type type)
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
                if (type.IsEquivalentTo(typeof(long)))
                {
                    if (value.TryParse(out long l))
                        return l;
                    else
                        return default;
                }
                else if (type.IsEquivalentTo(typeof(int)))
                {
                    if (value.TryParse(out int l))
                        return l;
                    else
                        return default;
                }
                else if (type.IsEquivalentTo(typeof(double)))
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
            else if (type.IsEquivalentTo(typeof(string)))
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
        private static bool TryParseEnum(Type type,string val,out object res)
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
        /// <summary>
        /// 从目标结构体制作类型<typeparamref name="T"/>的实例
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">目标值</param>
        /// <returns>类型为<typeparamref name="T"/>的实例，如果失败返回<see langword="default"/></returns>
        public static T Get<T>(this RedisValue value)
        {
            if (!value.HasValue || value.IsNull)
            {
                return default(T);
            }
            var type = typeof(T);
            if (type.IsPrimitive)
            {
                if (type.IsEquivalentTo(typeof(long)))
                {
                    if (value.TryParse(out long l))
                        return ((T)(object)l);
                    else
                        return default;
                }
                else if (type.IsEquivalentTo(typeof(int)))
                {
                    if (value.TryParse(out int l))
                        return ((T)(object)l);
                    else
                        return default;
                }
                else if (type.IsEquivalentTo(typeof(double)))
                {
                    if (value.TryParse(out double l))
                        return ((T)(object)l);
                    else
                        return default;
                }
                else
                {
                    try
                    {
                        var val = value.ToString();
                        var v = Convert.ChangeType(val, type);
                        return (T)v;
                    }
                    catch (Exception) { }
                }
            }
            else if (type.IsEquivalentTo(typeof(string)))
            {
                return ((T)(object)value.ToString());
            }
            else if (type.IsEnum)
            {
                if (TryParseEnum(type, value.ToString(), out var enu))
                {
                    return (T)enu;
                }
            }
            else if (type.IsClass || type.IsValueType)
            {
                var val = value.ToString();
                var obj = JsonSerializer.Deserialize(val, type);
                return (T)obj;
            }
            return default;
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
