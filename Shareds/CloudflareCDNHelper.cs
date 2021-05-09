using Anf.Platform.Services;

namespace Anf.CDN
{
    internal static class CloudflareCDNController
    {
        public static CloudflareCDNCacheFetcher GetChapterCDN()
        {
            return new CloudflareCDNCacheFetcher(CloudflareCDNHelper.ChaptersOptions);
        }
    }
    internal class CloudflareCDNHelper
    {
        private const int cacheTimeMs = 60 * 60;//1h
        private static readonly string Key = "QwoASRBdKFTruMsArPTsG6Vhv-TE8RX4Guirvg-q";
        private static readonly string UserIdentity = "0afb1d50681f4397ceba4a24fe7596c6";
        private static readonly string Email = "15218450198@163.com";

        private static readonly string ChapterKvNameSpace = "b8c23e13f6a942058f92a1a23ed4e4c7";

        public static CloudflareCDNOptions ChaptersOptions => new CloudflareCDNOptions
        {
            Key = Key,
            TTLMs = cacheTimeMs,
            Email = Email,
            NameSpaceId = ChapterKvNameSpace,
            UserId = UserIdentity
        };
    }
}
