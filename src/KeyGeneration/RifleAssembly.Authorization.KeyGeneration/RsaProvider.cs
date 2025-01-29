using System.Security.Cryptography;
using System.Text;

namespace RifleAssembly.Authorization.KeyGeneration
{
    internal class RsaProvider
    {
        public void Generate(string privateKeyPath, string publicKeyPath)
        {
            var rsaPrivate = new RSACryptoServiceProvider();
            var rsaPublic = new RSACryptoServiceProvider();
            rsaPrivate.PersistKeyInCsp = false;
            rsaPublic.PersistKeyInCsp = false;

            var privateKey = rsaPrivate.ToXmlString(true);
            var publicKey = rsaPublic.ToXmlString(false);

            using var privateKeyFile = File.Create(privateKeyPath);
            using var publicKeyFile = File.Create(publicKeyPath);

            privateKeyFile.Write(Encoding.UTF8.GetBytes(privateKey));
            publicKeyFile.Write(Encoding.UTF8.GetBytes(publicKey));
        }
    }
}
