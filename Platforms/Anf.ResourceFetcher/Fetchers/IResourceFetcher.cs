using System.Collections.Generic;
using System.Text;

namespace Anf.ResourceFetcher.Fetchers
{
    public interface IResourceFetcher : ISingleResourceFetcher, IBatchResourceFetcher
    {

    }
}
