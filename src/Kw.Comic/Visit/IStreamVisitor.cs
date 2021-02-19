using System.IO;

namespace Kw.Comic.Visit
{
    public interface IStreamVisitor : IResourceVisitor
    {
        Stream Stream { get; }
    }
}
