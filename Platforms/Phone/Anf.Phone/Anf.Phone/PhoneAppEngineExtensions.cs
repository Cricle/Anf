using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Anf.Easy;
using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Anf.Phone.Services;
using Anf.Platform;
using Anf.Platform.Services;
using Anf.Services;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace Anf.Phone
{
    public static class PhoneAppEngineExtensions
    {
        private static readonly string Workstation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static void AddPhone()
        {
            var store = FileStoreService.FromMd5Default(Path.Combine(Workstation, XComicConst.CacheFolderName));

            AppEngine.Services.AddSingleton<IComicSaver>(store);
            AppEngine.Services.AddSingleton<IStoreService>(store);
            AppEngine.Services.AddSingleton<IPlatformService, PlatformService>();
            AppEngine.Services.AddSingleton<IStreamImageConverter<ImageSource>, StreamImageConverter>();
            AppEngine.Services.AddSingleton<IResourceFactoryCreator<ImageSource>, PlatformResourceCreatorFactory<ImageSource>>();
            AppEngine.Services.AddSingleton<ExceptionService>();
        }
    }
}
