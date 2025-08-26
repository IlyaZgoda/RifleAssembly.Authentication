using RifleAssembly.WebService.SharedKernel.Result;

namespace RifleAssembly.Authentication.Web.Infrastructure.Services
{
    public interface ILdapService
    {
        Task<Result<string>> AuthenticateAsync(string login, string password);
    }
}