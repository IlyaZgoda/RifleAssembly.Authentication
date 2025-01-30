using RifleAssembly.Authorization.Web.Students;

namespace RifleAssembly.Authorization.Web.Services
{
    public class Ldap
    {
        private readonly TokenProvider _tokenProvider;

        public Ldap(TokenProvider tokenProvider) =>
            _tokenProvider = tokenProvider;

        public string? Authenticate(string login, string password)
        {
            return default;
        }
    }
}
