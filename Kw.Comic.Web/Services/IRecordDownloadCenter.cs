using Kw.Comic.Engine.Easy.Downloading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kw.Comic.Web.Services
{
    public interface IRecordDownloadCenter : IDownloadCenter, IReadOnlyDictionary<string, DownloadBox>
    {
        IEnumerable<ProcessInfo> ProcessInfos { get; }

        Task AddOrFromCacheAsync(string address);
    }
}
