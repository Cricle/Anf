using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Anf.ChannelModel.Helpers
{
    public static class RSAHelper
    {
        public const int RSAKeyLen = 2048;
        public static RSASecretKey GenerateRSASecretKey()
        {
            using (var rsa = new RSACryptoServiceProvider(RSAKeyLen))
            {
                return new RSASecretKey(rsa.ToXmlString(true), rsa.ToXmlString(false));
            }
        }
        public static string RSAEncrypt(string xmlPublicKey, string content)
        {
            string encryptedContent = string.Empty;
            using (var rsa = new RSACryptoServiceProvider(RSAKeyLen))
            {
                rsa.FromXmlString(xmlPublicKey);
                var encryptedData = rsa.Encrypt(Encoding.Default.GetBytes(content), false);
                encryptedContent = Convert.ToBase64String(encryptedData);
            }
            return encryptedContent;
        }
        public static string RSADecrypt(string xmlPrivateKey, string content)
        {
            try
            {
                string decryptedContent = string.Empty;
                using (var rsa = new RSACryptoServiceProvider(RSAKeyLen))
                {
                    rsa.FromXmlString(xmlPrivateKey);
                    var decryptedData = rsa.Decrypt(Convert.FromBase64String(content), false);
                    decryptedContent = Encoding.UTF8.GetString(decryptedData);
                }
                return decryptedContent;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
