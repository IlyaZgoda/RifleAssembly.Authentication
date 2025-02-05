namespace RifleAssembly.Authentication.Web.Infrastructure.Services
{
    public interface ILdapService
    {
        string? Authenticate(string login, string password);
    }
}