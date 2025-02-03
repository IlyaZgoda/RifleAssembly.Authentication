using RifleAssembly.Authorization.Web.Students;
using System.DirectoryServices;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;
using SearchResult = System.DirectoryServices.SearchResult;

namespace RifleAssembly.Authorization.Web.Services
{
    public class Ldap
    {
        private readonly TokenProvider _tokenProvider;

        private readonly string _ldapPath = "LDAP://stud.asu.ru";
        public Ldap(TokenProvider tokenProvider) =>
            _tokenProvider = tokenProvider;

        public string? Authenticate(string login, string password)
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
                Console.WriteLine($"does not connected {ex.Message} \n {ex.StackTrace}");

                return null;
            }
        }
    }
}
