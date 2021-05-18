using System.Threading.Tasks;

namespace Anf.Test
{
    internal class NullIProposalProvider : IProposalProvider
    {
        public string EngineName { get; set; }

        public int Take { get; set; }

        public Task<ComicSnapshot[]> GetProposalAsync(int take)
        {
            Take = take;
            return Task.FromResult<ComicSnapshot[]>(null);
        }
    }
}
