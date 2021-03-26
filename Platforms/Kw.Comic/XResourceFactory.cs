using Kw.Comic.Engine.Easy.Visiting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kw.Comic
{
    internal class XResourceFactory : IResourceFactory<ImageSource>
    {
        private readonly ResourceFactoryCreateContext<ImageSource> context;

        public XResourceFactory(ResourceFactoryCreateContext<ImageSource> context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Dispose()
        {
        }

        public async Task<ImageSource> GetAsync(string address)
        {
            using (var stream = await context.SourceProvider.GetImageStreamAsync(address))
            {
                var source = ImageSource.FromStream(() => stream);
                return source;
            }
        }
    }
}
