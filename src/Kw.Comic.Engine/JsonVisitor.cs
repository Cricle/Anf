using System;
using System.Collections.Generic;
using System.Text;
#if StandardLib
using System.Text.Json;
#else
using Newtonsoft.Json.Linq;
#endif

namespace Kw.Comic.Engine
{
#if StandardLib
    public struct JsonVisitor : IJsonVisitor
    {
        private readonly JsonElement doc;

        public JsonVisitor(JsonElement doc)
        {
            this.doc = doc;
        }

        public IJsonVisitor this[string key]
        {
            get => new JsonVisitor(doc.GetProperty(key));
        }

        public IEnumerable<IJsonVisitor> ToArray()
        {
            foreach (var item in doc.EnumerateArray())
            {
                yield return new JsonVisitor(item);
            }
        }

        public override string ToString()
        {
            return doc.ToString();
        }
    }
#else
    public struct JsonVisitor : IJsonVisitor
    {
        private readonly JToken @object;

        public JsonVisitor(JToken @object)
        {
            this.@object = @object;
        }

        public IJsonVisitor this[string key]
        {
            get => new JsonVisitor(key);
        }
        public IEnumerable<IJsonVisitor> ToArray()
        {
            foreach (var item in (JArray)@object)
            {
                yield return new JsonVisitor(item);
            }
        }
        public override string ToString()
        {
            return @object?.ToString();
        }
    }

#endif
}
