using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf.Managers
{
    [EnableService(ServiceLifetime = ServiceLifetime.Singleton)]
    public class DownloadManager : ComicPhysicalManager
    {
        public const string DownloadFolderName = "Downloads";
        public static string DownloadFolderPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DownloadFolderName);

        public DownloadManager() 
            : base(DownloadFolderPath)
        {
        }
    }
}
