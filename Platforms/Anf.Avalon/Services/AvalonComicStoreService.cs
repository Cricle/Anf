using Anf.Avalon.Models;
using Anf.Platform.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Avalon.Services
{
    public class AvalonComicStoreService : ComicStoreService<AvalonComicStoreBox>
    {
        public AvalonComicStoreService(DirectoryInfo folder, int cacheSize = 50) : base(folder, cacheSize)
        {
        }

        protected override AvalonComicStoreBox CreateBox(FileInfo file)
        {
            return new AvalonComicStoreBox(file);
        }
    }
}
