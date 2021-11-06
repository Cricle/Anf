using Anf.Easy;
using Anf.Easy.Store;
using Anf.Engine;
using Anf.Platform.Models;
using Anf.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Platform.Services
{
    public abstract class ComicStoreService<TStoreBox> : FileStoreService
        where TStoreBox:ComicStoreBox
    {
        public const string Extensions = "anfc";

        public const string Pattern = "*." + Extensions;

        protected ComicStoreService(DirectoryInfo folder, int cacheSize = 50)
            : base(folder, MD5AddressToFileNameProvider.Instance, cacheSize)
        {

        }

        public IEnumerable<FileInfo> EnumerableModelFiles()
        {
            return Folder.EnumerateFiles(Pattern);
        }
        public TStoreBox GetStoreBox(string address)
        {
            return GetStoreBoxes(new[] { address }).FirstOrDefault();
        }
        public IEnumerable<TStoreBox> GetStoreBoxes(IEnumerable<string> address)
        {
            var convertedAddress =new HashSet<string>(address.Select(x => PathHelper.EnsureName(x)),StringComparer.OrdinalIgnoreCase);
            foreach (var item in EnumerableModelFiles())
            {
                var fn = Path.GetFileNameWithoutExtension(item.Name);
                if (convertedAddress.Contains(fn))
                {
                    yield return CreateBox(item);
                }
            }
        }
        protected abstract TStoreBox CreateBox(FileInfo file);
        public string Store(ComicEntity entity,bool superFavorite=false)
        {
            var model = new ComicStoreModel
            {
                Chapters = entity.Chapters,
                ComicUrl = entity.ComicUrl,
                CreateTime = DateTime.Now.Ticks,
                ImageUrl = entity.ImageUrl,
                Name = entity.Name,
                Descript = entity.Descript,
                SuperFavorite= superFavorite
            };
           return Store(model);
        }
        public string Store(ComicStoreModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var str = JsonHelper.Serialize(model);
            var path = GetModelPath(model.ComicUrl);
            File.WriteAllText(path, str);
            return path;
        }
        private string GetModelPath(string address)
        {
            var fn = PathHelper.EnsureName(address) + "." + Extensions;
            var path = Path.Combine(Folder.FullName, fn);
            return path;
        }
        public void Clean()
        {
            foreach (var item in EnumerableModelFiles())
            {
                item.Delete();
            }
        }
        public bool Remove(string address)
        {
            var path = GetModelPath(address);
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
        public ComicStoreModel GetModel(string path)
        {
            var text = File.ReadAllText(path);
            var model = JsonHelper.Deserialize<ComicStoreModel>(text);
            return model;
        }
        public IEnumerable<ComicStoreModel> EnumerableModels(bool ignoreErrorFile = true, Action<FileInfo, Exception> errorCallback = null)
        {
            foreach (var item in EnumerableModelFiles())
            {
                ComicStoreModel model = null;
                try
                {
                    model = GetModel(item.FullName);
                }
                catch (Exception ex)
                {
                    errorCallback?.Invoke(item, ex);
                    if (!ignoreErrorFile)
                    {
                        throw;
                    }
                }
                if (model != null)
                {
                    yield return model;
                }
            }
        }
    }
}
