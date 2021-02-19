using System;

namespace Kw.Comic.Engine
{
    public class ComicSourceContext
    {
        public ComicSourceContext(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException($"“{nameof(source)}”不能是 Null 或为空。", nameof(source));
            }

            Source = source;
            Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out var uri);
            Uri = uri;
        }

        public ComicSourceContext(Uri uri)
        {
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            Source = uri.OriginalString;
        }

        public string Source { get; }

        public Uri Uri { get; }
    }
}
