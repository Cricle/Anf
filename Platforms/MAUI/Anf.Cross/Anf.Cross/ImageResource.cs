using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Cross
{
    public class ImageResource
    {
        private Stream stream;
        private ImageSource image;
        private readonly Func<ImageSource> imageFactory;
        private readonly Func<Stream> streamFactory;

        public ImageResource(string path, Stream stream, ImageSource image)
        {
            Path = path;
            this.stream = stream;
            this.image = image;
        }

        public ImageResource(string path, Func<ImageSource> imageFactory, Func<Stream> streamFactory)
        {
            Path = path;
            this.imageFactory = imageFactory;
            this.streamFactory = streamFactory;
        }

        public ImageResource(string path, Stream stream, Func<ImageSource> imageFactory)
        {
            Path = path;
            this.stream = stream;
            this.imageFactory = imageFactory;
        }

        public ImageResource(string path, ImageSource image, Func<Stream> streamFactory)
        {
            Path = path;
            this.image = image;
            this.streamFactory = streamFactory;
        }

        public string Path { get; }

        public ImageSource Image
        {
            get
            {
                if (image == null)
                {
                    image = imageFactory?.Invoke();
                }
                return image;
            }
        }

        public Stream Stream
        {
            get
            {
                if (stream == null)
                {
                    stream = streamFactory?.Invoke();
                }
                return stream;
            }
        }
        public static implicit operator ImageSource(ImageResource res)
        {
            return res.Image;
        }
        public static implicit operator Stream(ImageResource res)
        {
            return res.Stream;
        }
    }
}
