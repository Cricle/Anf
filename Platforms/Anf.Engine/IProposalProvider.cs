using System.Threading.Tasks;

namespace Anf
{
    public interface IProposalProvider : IEngine
    {
        Task<ComicSnapshot[]> GetProposalAsync(int take);
    }
}
