using Anf.Networks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Anf.Platform.Models.Impl
{
    public class WithImageComicSnapshotInfo<TImage> : WithImageStorableComicSnapshotInfo<TImage>, IDisposable
    {
        public WithImageComicSnapshotInfo(ComicSnapshot snapshot, INetworkAdapter networkAdapter)
            : base(snapshot)
        {
            this.networkAdapter = networkAdapter;
            InitLogoImage();
        }
        private readonly INetworkAdapter networkAdapter;
        private TImage logoImage;

        public TImage LogoImage
        {
            get { return logoImage; }
            private set => Set(ref logoImage, value);
        }


        public bool DoNotDisposeLogoImage { get; set; }

        private async void InitLogoImage()
        {
            try
            {
                LogoImage = await StoreFetchHelper.GetOrFromCacheAsync<TImage>(Snapshot.ImageUri);
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
