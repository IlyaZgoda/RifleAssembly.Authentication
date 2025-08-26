using RifleAssembly.Authentication.Web.Errors;
using RifleAssembly.Authentication.Web.Exceptions;
using RifleAssembly.Authentication.Web.Students;
using RifleAssembly.WebService.SharedKernel.Result;
using RifleAssembly.WebService.SharedKernel.Result.Errors;
using System.DirectoryServices;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;
using SearchResult = System.DirectoryServices.SearchResult;

namespace RifleAssembly.Authentication.Web.Infrastructure.Services
{
    public class LdapWindows : ILdapService
    {
        private readonly TokenProvider _tokenProvider;
        private readonly ILogger<LdapWindows> _logger;
        private readonly string _ldapPath = "LDAP://stud.asu.ru";
        public LdapWindows(TokenProvider tokenProvider, ILogger<LdapWindows> logger)
        {
            _tokenProvider = tokenProvider;
            _logger = logger;
        }

        public async Task<Result<string>> AuthenticateAsync(string login, string password)
        {
            try
            {
                using DirectoryEntry directoryEntry = new(_ldapPath, login, password);

                object native = directoryEntry.NativeObject;

                DirectorySearcher directorySearcher = new(directoryEntry);

                string filter = $"(&(objectClass=user)(sAMAccountName={login}))";

                directorySearcher.Filter = filter;

                SearchResult searchResult = directorySearcher.FindOne();

                var desription = searchResult.Properties["description"];
                var descriptionSplit = desription[0].ToString().Split(',', 2, StringSplitOptions.None);

                var groupTitle = descriptionSplit[0].Remove(0, 7);
                var instituteTitle = descriptionSplit[1].Trim();

                var fullName = searchResult.Properties["cn"];
                var fullNameSplit = fullName[0].ToString().Split(' ');

                var lastName = fullNameSplit[0];
                var firstName = fullNameSplit[1];
                var middleName = fullNameSplit[2];

                var student = new Student(instituteTitle, groupTitle, firstName, lastName, middleName, login);
                var token = _tokenProvider.Create(student);

                return token;
            }
            catch (Exception ex)
            {
                throw new LdapUnavailableException(LdapErrors.ServerUnavailable, ex);
            }
        }
    }
}
