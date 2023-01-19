using System;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    public class DelegateResourceFactory<T> : IResourceFactoryCreator<T>
    {
        public DelegateResourceFactory(Func<ResourceFactoryCreateContext<T>, Task<IResourceFactory<T>>> creator)
        {
            Creator = creator ?? throw new ArgumentNullException(nameof(creator));
        }

        public Func<ResourceFactoryCreateContext<T>, Task<IResourceFactory<T>>> Creator { get; }
        public Task<IResourceFactory<T>> CreateAsync(ResourceFactoryCreateContext<T> context)
        {
            return Creator(context);
        }
    }
}
