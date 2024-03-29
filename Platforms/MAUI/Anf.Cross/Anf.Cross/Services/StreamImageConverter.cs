﻿using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Cross.Services
{
    internal class StreamImageConverter : IStreamImageConverter<ImageSource>
    {
        public Task<ImageSource> ToImageAsync(Stream stream)
        {
            var img = ImageSource.FromStream(() => stream);
            return Task.FromResult(img);
        }
    }
    internal class StreamResourceConverter : IStreamImageConverter<ImageResource>
    {
        public Task<ImageResource> ToImageAsync(Stream stream)
        {
            var resource = new ImageResource(null, stream, () => ImageSource.FromStream(() => stream));
            return Task.FromResult(resource);
        }
    }
}
