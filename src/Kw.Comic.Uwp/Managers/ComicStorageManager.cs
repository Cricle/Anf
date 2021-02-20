using Kw.Comic.Uwp.Models;
using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace Kw.Comic.Uwp.Managers
{
    [EnableService(ServiceLifetime = ServiceLifetime.Singleton)]
    public class ComicStorageManager : IDisposable
    {
        private Stream fileStream;

        public bool IsOpen => fileStream != null;

        public List<ComicStorage> ComicStorages { get; }

        public async Task<bool> SaveAsync()
        {
            if (fileStream != null)
            {
                fileStream.SetLength(0);
                fileStream.Flush();
                await JsonSerializer.SerializeAsync(fileStream, ComicStorages);
                return true;
            }
            return false;
        }
        public async Task OpenAsync()
        {
            fileStream?.Dispose();
            var sf = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(ComicConst.StorageComicFolder, CreationCollisionOption.OpenIfExists);
            var file = await sf.CreateFileAsync(ComicConst.StorageComicInfosFile, CreationCollisionOption.OpenIfExists);
            var fs = await file.OpenReadAsync();
            fileStream = fs.AsStream();
        }

        public async Task<bool> LoadComicsAsync()
        {
            if (fileStream!=null)
            {
                try
                {
                    var datas = await JsonSerializer.DeserializeAsync<ComicStorage[]>(fileStream);
                    ComicStorages.AddRange(datas);
                    return true;
                }
                catch (Exception) { }
            }
            return false;
        }

        public void Dispose()
        {
            fileStream?.Dispose();
            fileStream = null;
        }
    }
}
