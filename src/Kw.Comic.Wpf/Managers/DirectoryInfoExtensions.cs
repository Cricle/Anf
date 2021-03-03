using System.IO;

namespace Kw.Comic.Wpf.Managers
{
    internal static class DirectoryInfoExtensions
    {
        public static bool? EnsureFolderCreated(this DirectoryInfo info)
        {
            if (info.Exists)
            {
                return null;
            }
            info.Create();
            return true;
        }
    }
}
