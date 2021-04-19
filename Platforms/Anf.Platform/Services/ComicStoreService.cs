﻿using Anf.Easy;
using Anf.Easy.Store;
using Anf.Platform.Models;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
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
    public class ComicStoreService : FileStoreService
    {
        public const string Extensions = "anfc";

        public const string Pattern = "*." + Extensions;

        public ComicStoreService(DirectoryInfo folder, int cacheSize = 50)
            : base(folder, MD5AddressToFileNameProvider.Instance, cacheSize)
        {

        }
        public IEnumerable<FileInfo> EnumerableModelFiles()
        {
            return Folder.EnumerateFiles(Pattern);
        }
        public ComicStoreBox GetStoreBox(string address)
        {
            return GetStoreBoxes(new[] { address }).FirstOrDefault();
        }
        public IEnumerable<ComicStoreBox> GetStoreBoxes(IEnumerable<string> address)
        {
            var convertedAddress =new HashSet<string>(address.Select(x => MD5AddressToFileNameProvider.Instance.Convert(x)),StringComparer.OrdinalIgnoreCase);
            foreach (var item in EnumerableModelFiles())
            {
                if (convertedAddress.Contains(item.Name))
                {
                    yield return new ComicStoreBox(item);
                }
            }
        }
        public void Store(ComicEntity entity)
        {
            var model = new ComicStoreModel
            {
                Chapters = entity.Chapters,
                ComicUrl = entity.ComicUrl,
                CreateTime = DateTime.Now.Ticks,
                ImageUrl = entity.ImageUrl,
                Name = entity.Name,
                Descript = entity.Descript,
            };
            Store(model);
        }
        public void Store(ComicStoreModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var str = JsonConvert.SerializeObject(model);
            var path = GetModelPath(model.ComicUrl);
            File.WriteAllText(path, str);
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
        public bool Remove(string name)
        {
            var path = GetModelPath(name);
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
            var model = JsonConvert.DeserializeObject<ComicStoreModel>(text);
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
