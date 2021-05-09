using Anf.Phone.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Anf.Platform;

namespace Anf.Phone
{
    public class App : Application
    {
        private readonly Stream logXml;

        public App(Stream logXml)
        {
            this.logXml = logXml;
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (MainPage is null)
            {
                AppEngine.Reset();

                AppEngine.AddServices();
                AppEngine.Services.AddPhoneService(logXml);
                MainPage = new MainPage();
                StoreFetchHelper.DefaultStoreFetchSettings.DisposeStream = false;
            }
        }
    }
}
