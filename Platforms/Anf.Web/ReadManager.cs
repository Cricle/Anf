using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web
{
    internal class ReadManager
    {
        private readonly SharedComicVisiting sharedComicVisiting;
        private readonly ConcurrentDictionary<string, VisitingViewModel<Stream, Stream>> readingMap;

        public IReadOnlyCollection<string> IncludeConnectionId => readingMap.Keys.ToArray();
        public int Count => readingMap.Count;

        public ReadManager(SharedComicVisiting sharedComicVisiting)
        {
            this.sharedComicVisiting = sharedComicVisiting;
            readingMap = new ConcurrentDictionary<string, VisitingViewModel<Stream, Stream>>();
        }
        public VisitingViewModel<Stream, Stream> Connect(string connectionId)
        {
            var box= readingMap.GetOrAdd(connectionId, _ => new VisitingViewModel<Stream, Stream>());
            return box;
        }

        public void Disconnect(string connectionId)
        {
            if (readingMap.TryRemove(connectionId,out var box))
            {
                box.Dispose();
            }
        }
    }
}
