using Kw.Comic.Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kw.Comic.Wpf.Managers
{
    public class ComicPhysicalManager
    {
        public const string FolderName = "Physical";

        public ComicPhysicalManager(string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                throw new ArgumentException($"“{nameof(basePath)}”不能是 Null 或为空", nameof(basePath));
            }

            BasePath = basePath;
            Folder = new DirectoryInfo(Path.Combine(BasePath, FolderName));
        }
        private char invalidReplaceChar = PathHelper.DefaultInvalidReplaceChar;

        public string BasePath { get; }

        public DirectoryInfo Folder { get; }

        public char InvalidReplaceChar
        {
            get => invalidReplaceChar;
            set
            {
                if (PathHelper.IsInvalidChar(value))
                {
                    throw new ArgumentException($"{value} is invalid char!");
                }
                invalidReplaceChar = value;
            }
        }
        public void Clear()
        {
            Folder.Delete(true);
        }
        public PhysicalComicInfo AddComic(ComicEntity entity)
        {
            Folder.EnsureFolderCreated();
            var name = PathHelper.EnsureName(entity.Name);
            var path = Path.Combine(Folder.FullName, name);
            var dirInfo = new DirectoryInfo(path);
            dirInfo.EnsureFolderCreated();
            var fn = $"{name}.{PhysicalComicInfo.Extensions}";
            var fPath = Path.Combine(path, fn);
            var fileInfo = new FileInfo(fPath);
            if (!fileInfo.Exists)
            {
                var content = JsonConvert.SerializeObject(entity);
                File.WriteAllText(fPath, content);
            }
            return new PhysicalComicInfo(fileInfo, entity, dirInfo);
        }
        public IEnumerable<PhysicalComicInfo> EnumerableComic()
        {
            Folder.EnsureFolderCreated();
            var folders = Folder.EnumerateDirectories();
            foreach (var item in folders)
            {
                var file = item.EnumerateFiles($"*.{PhysicalComicInfo.Extensions}", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (file!=null)
                {
                    PhysicalComicInfo info = null;
                    try
                    {
                        using (var fs = file.OpenRead())
                        using (var sr = new StreamReader(fs))
                        {
                            var str = sr.ReadToEnd();
                            var inst = JsonConvert.DeserializeObject<ComicEntity>(str);
                            var dirName = PathHelper.EnsureName(inst.Name);
                            var dirPath = Path.Combine(item.FullName, dirName);
                            var dirInfo = new DirectoryInfo(dirPath);
                            info = new PhysicalComicInfo(file, inst, dirInfo);
                        }
                    }
                    catch (Exception) { }
                    if (info!=null)
                    {
                        yield return info;
                    }
                }
            }
        }
    }
}
