namespace Anf.ChannelModel.Helpers
{
    public readonly struct RSASecretKey
    {
        public RSASecretKey(string privateKey, string publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
        public readonly string PublicKey;
        public readonly string PrivateKey;

        public override string ToString()
        {
            return string.Format(
                "PrivateKey: {0}\r\nPublicKey: {1}", PrivateKey, PublicKey);
        }
    }
}
