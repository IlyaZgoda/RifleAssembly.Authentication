using System.Security.Cryptography;
using System.Text;

namespace RifleAssembly.Authorization.KeyGeneration
{
    internal class RsaProvider
    {
        public void Generate(string privateKeyPath, string publicKeyPath)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            rsa.PersistKeyInCsp = false;

            var privateKey = rsa.ToXmlString(true);
            var publicKey = rsa.ToXmlString(false);

            using var privateKeyFile = File.Create(privateKeyPath);
            using var publicKeyFile = File.Create(publicKeyPath);

            privateKeyFile.Write(Encoding.UTF8.GetBytes(privateKey));
            publicKeyFile.Write(Encoding.UTF8.GetBytes(publicKey));
        }
    }
}
