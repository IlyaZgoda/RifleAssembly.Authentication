using System.Security.Cryptography;

namespace RifleAssembly.Authorization.KeyGeneration
{
    internal class RsaProvider
    {
        public void Generate(string privateKeyPath, string publicKeyPath)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;

                var privateKey = rsa.ToXmlString(true);
                File.WriteAllText(privateKeyPath, privateKey);

                var publicKey = rsa.ToXmlString(false);
                File.WriteAllText(publicKeyPath, publicKey);
            }
        }
    }
}
