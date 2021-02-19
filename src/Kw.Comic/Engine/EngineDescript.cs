using System;
using System.Collections.Generic;
using System.Text;

namespace Kw.Comic.Engine
{
    public class EngineDescript : Dictionary<string, string>
    {
        public new string this[string key]
        {
            get { return GetOrDefault(key); }
            set
            {
                this[key] = value;
            }
        }
        public string GetOrDefault(string key)
        {
            if (TryGetValue(key, out var val))
            {
                return val;
            }
            return string.Empty;
        }
    }
}
