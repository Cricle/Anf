using System;
using System.Collections.Generic;
using System.Text;
#if StandardLib
using System.Text.Json;
#else
using Newtonsoft.Json.Linq;
#endif

namespace Anf
{
#if StandardLib
    public struct JsonVisitor : IJsonVisitor
    {
        private readonly JsonElement doc;
        private readonly JsonDocument docx;

        public JsonVisitor(JsonElement doc, JsonDocument docx)
        {
            this.doc = doc;
            this.docx = docx;
        }

        public IJsonVisitor this[string key]
        {
            get => new JsonVisitor(doc.GetProperty(key), docx);
        }

        public IEnumerable<IJsonVisitor> ToArray()
        {
            foreach (var item in doc.EnumerateArray())
            {
                yield return new JsonVisitor(item, docx);
            }
        }

        public override string ToString()
        {
            return doc.ToString();
        }
        public static IJsonVisitor FromString(string txt)
        {
            var doc = JsonDocument.Parse(txt);
            return new JsonVisitor(doc.RootElement, doc);
        }

        public void Dispose()
        {
            docx.Dispose();
        }
    }
#else
    public struct JsonVisitor : IJsonVisitor
    {
        private readonly JToken @object;

        public JsonVisitor(JToken @object)
        {
            this.@object = @object ?? throw new ArgumentNullException(nameof(@object));
        }

        public IJsonVisitor this[string key]
        {
            get => new JsonVisitor(@object[key]);
        }

        public void Dispose()
        {
        }
        public static IJsonVisitor FromString(string txt)
        {
            JToken tk;
            if (txt.StartsWith("{"))
            {
                tk = JObject.Parse(txt);
            }
            else
            {
                tk = JArray.Parse(txt);
            }
            return new JsonVisitor(tk);
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
