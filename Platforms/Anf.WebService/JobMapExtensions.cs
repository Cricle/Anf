using System;
using System.Collections.Generic;

namespace Quartz
{
    public static class JobMapExtensions
    {
        public static T GetAs<T>(this IDictionary<string, object> map, string key)
        {
            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"“{nameof(key)}”不能是 Null 或为空。", nameof(key));
            }

            if (map.TryGetValue(key, out var oprovider)
                && oprovider is T provider)
            {
                return provider;
            }
            return default;
        }
    }
}
