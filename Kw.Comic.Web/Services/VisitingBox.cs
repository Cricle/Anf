using Kw.Comic.Engine.Easy.Visiting;
using System.Threading.Tasks;

namespace KwC.Services
{
    public class VisitingBox
    {
        public VisitingBox(string address, IComicVisiting<string> visiting)
        {
            Address = address;
            Visiting = visiting;
        }

        public string Address { get; }

        public IComicVisiting<string> Visiting { get; }

        public Task LoadAsync()
        {
            if (Visiting.Address != Address)
            {
                return Visiting.LoadAsync(Address);
            }
            return Task.CompletedTask;
        }
    }
}
