using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml;

namespace Anf.ChannelModel.Helpers
{
    public static class RSAHelper
    {
        public const int RSAKeyLen = 2048;
        public static RSASecretKey GenerateRSASecretKey()
        {
            var rsa = new RSA();
            var key=rsa.GetKey(RSAKeyLen);
			return new RSASecretKey(key.PrivateKey, key.PublicKey);
		}
        public static string RSAEncrypt(string xmlPublicKey, string content)
        {
            var rsa = new RSA();
			return rsa.EncryptByPublicKey(content, xmlPublicKey);
        }
        public static string RSADecrypt(string xmlPrivateKey, string content)
        {
            try
            {
                var rsa = new RSA();
                return rsa.DecryptByPrivateKey(content, xmlPrivateKey);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
