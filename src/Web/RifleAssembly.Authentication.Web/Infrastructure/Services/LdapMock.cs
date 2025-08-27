using RifleAssembly.Authentication.Web.Errors;
using RifleAssembly.Authentication.Web.Students;
using RifleAssembly.WebService.SharedKernel.Result;

namespace RifleAssembly.Authentication.Web.Infrastructure.Services
{
    public class LdapMock : ILdapService
    {
        private readonly TokenProvider _tokenProvider;

        public LdapMock(TokenProvider tokenProvider) =>
            _tokenProvider = tokenProvider;

        public async Task<Result<string>> AuthenticateAsync(string login, string password)
        {
            return await Task.FromResult(Result.Failure<string>(LdapErrors.InvalidCredentials));
            var student = new Student("Имит", "4.205-1", "Илья", "Згода", "Константинович", "Zgoda.51052");
            var token = _tokenProvider.Create(student);
            return await Task.FromResult(Result.Success(token));
        }
    }
}
