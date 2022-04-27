#if NETSTANDARD2_0_OR_GREATER||NETCOREAPP2_0_OR_GREATER||NET461_OR_GREATER
#define USING_TEXT_JSON
#endif
using System;
using System.Collections;
using System.Collections.Generic;
#if USING_TEXT_JSON
using System.Text.Json;
#else
using Newtonsoft.Json.Linq;
#endif

namespace Anf
{
#if USING_TEXT_JSON
    public struct JsonVisitor : IJsonVisitor
    {
        private readonly JsonElement doc;
        private readonly JsonDocument docx;

        public bool IsArray => doc.ValueKind == JsonValueKind.Array;

        public bool HasValue => doc.ValueKind != JsonValueKind.Null;

        public JsonVisitor(object obj)
        {
            var str = JsonSerializer.Serialize(obj);
            docx = JsonDocument.Parse(str);
            doc = docx.RootElement;
        }
        public JsonVisitor(JsonElement doc, JsonDocument docx)
        {
            this.doc = doc;
            this.docx = docx;
        }

        public IJsonVisitor this[string key]
        {
            get => new JsonVisitor(doc.GetProperty(key), docx);
        }

        public IEnumerable<IJsonVisitor> ToEnumerable()
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
            JsonDocument doc;
            if (string.IsNullOrEmpty(txt))
            {
                doc = JsonDocument.Parse("null");
            }
            else
            {
                doc = JsonDocument.Parse(txt);
            }
            return new JsonVisitor(doc.RootElement, doc);
        }

        public void Dispose()
        {
            docx.Dispose();
        }

        public IEnumerator<KeyValuePair<string, IJsonVisitor>> GetEnumerator()
        {
            foreach (var item in doc.EnumerateObject())
            {
                yield return new KeyValuePair<string, IJsonVisitor>(
                    item.Name, new JsonVisitor(item.Value, docx));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
#else
    public struct JsonVisitor : IJsonVisitor
    {
        private readonly JToken @object;

        public bool IsArray
        {
            get
            {
                ThrowIfNullValue();
                return @object.Type == JTokenType.Array;
            }
        }

        public bool HasValue => @object != null && @object.Type != JTokenType.Null;

        public JsonVisitor(JToken @object)
        {
            this.@object = @object;
        }
        public JsonVisitor(object @object)
        {
            if (@object is null)
            {
                this.@object = null;
            }
            else if (@object.GetType().IsArray)
            {
                this.@object = JArray.FromObject(@object);
            }
            else
            {
                this.@object = JObject.FromObject(@object);
            }
        }

        public IJsonVisitor this[string key]
        {
            get
            {
                ThrowIfNullValue();
                return new JsonVisitor(@object[key]);
            }
        }

        public void Dispose()
        {

        }
        public static IJsonVisitor FromString(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                return new JsonVisitor(null);
            }
            JToken tk;
            if (txt[0] == '{')
            {
                tk = JObject.Parse(txt);
            }
            else
            {
                tk = JArray.Parse(txt);
            }
            return new JsonVisitor(tk);
        }

        public IEnumerable<IJsonVisitor> ToEnumerable()
        {
            ThrowIfNullValue();

            foreach (var item in (JArray)@object)
            {
                yield return new JsonVisitor(item);
            }
        }
        public override string ToString()
        {
            ThrowIfNullValue();
            return @object?.ToString();
        }
        private void ThrowIfNullValue()
        {
            if (@object == null)
            {
                throw new InvalidOperationException("The value is null");
            }
        }
        public IEnumerator<KeyValuePair<string, IJsonVisitor>> GetEnumerator()
        {
            ThrowIfNullValue();
            foreach (var item in @object)
            {
                var visitor = new JsonVisitor(item);
                var name = string.Empty;
                IJsonVisitor pvisitor = visitor;
                if (item is JProperty prop)
                {
                    name = prop.Name;
                    pvisitor = new JsonVisitor(prop.Value);
                }
                yield return new KeyValuePair<string, IJsonVisitor>(
                    name, pvisitor);

            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
#endif
}
