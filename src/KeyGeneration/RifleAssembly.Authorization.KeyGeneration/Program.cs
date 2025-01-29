namespace RifleAssembly.Authorization.KeyGeneration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var rsa = new RsaProvider();

            string projectPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.Parent!.FullName;
            string targetSolutionPath = Path.Combine(projectPath, "Web\\RifleAssembly.Authorization.Web\\Keys\\");
            string privateKeyPath = Path.Combine(targetSolutionPath, "private_key.xml");
            string publicKeyPath = Path.Combine(targetSolutionPath, "public_key.xml");

            rsa.Generate(privateKeyPath, publicKeyPath);
        }
    }
}
