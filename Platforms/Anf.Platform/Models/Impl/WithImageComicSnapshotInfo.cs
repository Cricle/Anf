using Anf.Networks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Anf.Platform.Models.Impl
{
    public class WithImageComicSnapshotInfo<TResource,TImage> : WithImageStorableComicSnapshotInfo<TResource,TImage>, IDisposable
    {
        public WithImageComicSnapshotInfo(ComicSnapshot snapshot, INetworkAdapter networkAdapter)
            : base(snapshot)
        {
            InitLogoImage();
        }
        private TImage logoImage;

        public TImage LogoImage
        {
            get { return logoImage; }
            private set => SetProperty(ref logoImage, value);
        }


        public bool DoNotDisposeLogoImage { get; set; }

        private async void InitLogoImage()
        {
            try
            {
                LogoImage = await StoreFetchHelper.GetOrFromCacheAsync<TResource,TImage>(Snapshot.ImageUri);
            }
            catch (Exception)
            {
                //TODO: null img
            }
        }

        public void Dispose()
        {
            if (!DoNotDisposeLogoImage&&LogoImage is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
