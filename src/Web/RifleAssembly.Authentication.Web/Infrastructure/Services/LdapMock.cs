using RifleAssembly.Authentication.Web.Students;

namespace RifleAssembly.Authentication.Web.Infrastructure.Services
{
    public class LdapMock : ILdapService
    {
        private readonly TokenProvider _tokenProvider;

        public LdapMock(TokenProvider tokenProvider) =>
            _tokenProvider = tokenProvider;

        public string? Authenticate(string login, string password)
        {
            var student = new Student("Имит", "4.205-1", "Илья", "Згода", "Константинович", "Zgoda.51052");
            var token = _tokenProvider.Create(student);

            return token;
        }
    }
}
