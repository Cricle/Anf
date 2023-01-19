using System;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    public class DelegateResourceFactoryCreator<TResource> : IResourceFactoryCreator<TResource>
    {
        public DelegateResourceFactoryCreator(Func<ResourceFactoryCreateContext<TResource>, Task<IResourceFactory<TResource>>> @delegate)
        {
            Delegate = @delegate ?? throw new ArgumentNullException(nameof(@delegate));
        }

        public Func<ResourceFactoryCreateContext<TResource>, Task<IResourceFactory<TResource>>> Delegate { get; }
        public Task<IResourceFactory<TResource>> CreateAsync(ResourceFactoryCreateContext<TResource> context)
        {
            return Delegate(context);
        }
    }
}
