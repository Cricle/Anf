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
        public static readonly string DrakMoelKey = ConfigurationPath.Combine(SectionKey, nameof(EnableDarkModel));
        public static readonly string AcrylicBlurKey = ConfigurationPath.Combine(SectionKey, nameof(EnableAcrylicBlur));

        private readonly ThemeService themeService;
        private readonly IConfigurationRoot configuration;

        public AnfSetting(ThemeService themeService, IConfigurationRoot configuration)
        {
            this.themeService = themeService;
            this.configuration = configuration;
        }


        public bool EnableDarkModel
        {
            get => configuration.GetValue<bool>(DrakMoelKey);
            set
            {
                themeService.CurrentModel = value ? FluentThemeMode.Dark : FluentThemeMode.Light;
            }
        }

        public bool EnableAcrylicBlur
        {
            get => configuration.GetValue<bool>(AcrylicBlurKey);
            set
            {
                themeService.EnableAcrylicBlur = value;
            }
        }
        private object tk;
        public async void Save()
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
                    settingObj[nameof(EnableDarkModel)] = EnableDarkModel;
                    settingObj[nameof(EnableAcrylicBlur)] = EnableAcrylicBlur;
                    var str = data.ToString();
                    File.WriteAllText(XComicConst.SettingFileFolder, str);
                }
                catch (Exception) { }
            }
        }
    }

}
