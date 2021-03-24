using Kw.Comic.Engine;
using System.Threading;

namespace Kw.Comic.Web.Services
{
    public class ProcessChangedInfo
    {
        private int current;

        public string Sign { get; set; }
        public int Current
        {
            get => current;
            set => Interlocked.Exchange(ref current, value);
        }
        public int Total { get; set; }

        public void CurrentInc()
        {
            Interlocked.Increment(ref current);
        }
    }
    public class ProcessInfo : ProcessChangedInfo
    {
        public ComicDetail Detail { get; set; }

        public string EngineName { get; set; }
    }
}
