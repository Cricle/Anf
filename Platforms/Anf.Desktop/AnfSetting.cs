using Anf.Desktop.Services;
using Ao.SavableConfig;
using Avalonia;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Desktop
{
    internal class AnfSetting : ObservableObject
    {
        public static readonly string SectionKey = "RuntimeSettings";
        public static readonly string EnableDarkModelKey = "EnableDarkModel";
        public static readonly string EnableAcrylicBlurKey = "EnableAcrylicBlur";
        public static readonly string DrakMoelKey = ConfigurationPath.Combine(SectionKey, EnableDarkModelKey);
        public static readonly string AcrylicBlurKey = ConfigurationPath.Combine(SectionKey, EnableAcrylicBlurKey);

        private object tk;

        public async void Save(ThemeService themeService)
        {
            var s = tk;
            await Task.Delay(1000);
            if (Interlocked.CompareExchange(ref tk, new object(), s) == s)
            {
                try
                {
                    var data = new JObject();
                    var settingObj = new JObject();
                    data[SectionKey] = settingObj;
                    settingObj[EnableDarkModelKey] = themeService.CurrentModel== FluentThemeMode.Dark;
                    settingObj[EnableAcrylicBlurKey] = themeService.EnableAcrylicBlur;
                    var str = data.ToString();
                    File.WriteAllText(XComicConst.SettingFileFolder, str);
                }
                catch (Exception) { }
            }
        }
    }

}
