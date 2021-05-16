namespace Anf.ChannelModel.Helpers
{
    public readonly struct RSAKey
    {
        /// <summary>
        /// 公钥
        /// </summary>
        public readonly string PublicKey;
        /// <summary>
        /// 私钥
        /// </summary>
        public readonly string PrivateKey;

        public RSAKey(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
    }
}
