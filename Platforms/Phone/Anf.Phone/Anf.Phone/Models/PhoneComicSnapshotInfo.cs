using Anf.Platform;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace Anf.Phone.Models
{
    public class PhoneComicSnapshotInfo : PhoneStorableComicSnapshotInfo, IDisposable
    {
        public PhoneComicSnapshotInfo(ComicSnapshot snapshot, HttpClient httpClient)
            : base(snapshot)
        {
            this.httpClient = httpClient;
            InitLogoImage();
        }
        private readonly HttpClient httpClient;
        private ImageSource logoImage;

        public ImageSource LogoImage
        {
            get { return logoImage; }
            private set => Set(ref logoImage, value);
        }

        private async void InitLogoImage()
        {
            try
            {
                LogoImage = await StoreFetchHelper.GetOrFromCacheAsync<ImageSource>(Snapshot.ImageUri);
            }
            catch (Exception)
            {
                //TODO: null img
            }
        }

        public void Dispose()
        {
            if (LogoImage is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
