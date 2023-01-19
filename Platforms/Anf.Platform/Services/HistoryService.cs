using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Platform.Services
{
    public class HistoryService : IDisposable
    {
        public static readonly TimeSpan DefaultLazyTime = TimeSpan.FromSeconds(1);

        public const string HistoryFileName = "Historys.anfh";

        public HistoryService(Stream storeStream)
        {
            StoreStream = storeStream ?? throw new ArgumentNullException(nameof(storeStream));
            if (!storeStream.CanSeek || !storeStream.CanRead || !storeStream.CanWrite)
            {
                throw new ArgumentException("Stream must can seek, read and write");
            }
            Lines = new ObservableCollection<string>();
            ReadFromStream();
            Lines.CollectionChanged += OnLinesCollectionChanged;
        }

        private async void OnLinesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await LazyWriteAsync(DefaultLazyTime);
        }

        private readonly SemaphoreSlim writeLocker=new SemaphoreSlim(1);
        private object writeToken=new object();

        public bool LeaveCloseStream { get; set; }

        public Stream StoreStream { get; }

        public int MaxLine { get; set; } = 10;

        public ObservableCollection<string> Lines { get; }

        public int SkipCount
        {
            get
            {
                var sk = Lines.Count - MaxLine;
                if (sk<0)
                {
                    return 0;
                }
                return sk;
            }
        }

        public async Task<bool> LazyWriteAsync(TimeSpan delayTime)
        {
            var tk = writeLocker;
            await Task.Delay(delayTime);
            if (Interlocked.CompareExchange(ref writeToken, new object(), tk) == tk)
            {
                await WriteAsync();
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            if (LeaveCloseStream)
            {
                StoreStream.Dispose();
            }
            Lines.CollectionChanged -= OnLinesCollectionChanged;
        }
        public void Write()
        {
            writeLocker.Wait();
            try
            {
                StoreStream.SetLength(0);
                var sw = new StreamWriter(StoreStream);
                foreach (var item in Lines.Skip(SkipCount).Distinct())
                {
                    sw.WriteLine(item);
                }
            }
            finally
            {
                writeLocker.Release();
            }
        }
        public async Task WriteAsync()
        {
            await writeLocker.WaitAsync();
            try
            {
                StoreStream.SetLength(0);
                var sw = new StreamWriter(StoreStream);
                foreach (var item in Lines.Skip(SkipCount).Distinct())
                {
                    await sw.WriteLineAsync(item);
                }
            }
            finally
            {
                writeLocker.Release();
            }
        }
        public void ReadFromStream()
        {
            foreach (var item in EnumerableLines())
            {
                Lines.Add(item);
            }
        }

        public IEnumerable<string> EnumerableLines()
        {
            StoreStream.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(StoreStream);
            string str;
            while ((str = sr.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    yield return str;
                }
            }
        }
        public static HistoryService FromFile(string path,FileMode mode= FileMode.OpenOrCreate)
        {
            return new HistoryService(File.Open(path, mode));
        }
    }
}
