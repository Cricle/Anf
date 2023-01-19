using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace Anf
{
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
            if (doc.ValueKind == JsonValueKind.Array)
            {
                var i = 0;
                foreach (var item in doc.EnumerateArray())
                {
                    yield return new KeyValuePair<string, IJsonVisitor>(i++.ToString(),
                        new JsonVisitor(item));
                }
            }
            else
            {
                foreach (var item in doc.EnumerateObject())
                {
                    yield return new KeyValuePair<string, IJsonVisitor>(
                        item.Name, new JsonVisitor(item.Value, docx));
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
