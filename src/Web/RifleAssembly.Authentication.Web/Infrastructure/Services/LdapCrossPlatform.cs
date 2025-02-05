using Novell.Directory.Ldap;
using RifleAssembly.Authentication.Web.Students;

namespace RifleAssembly.Authentication.Web.Infrastructure.Services
{
    public class LdapCrossPlatform : ILdapService
    {
        private readonly TokenProvider _tokenProvider;
        private readonly string _ldapHost = "stud.asu.ru";
        private readonly int _ldapPort = 389; // Стандартный порт LDAP

        public LdapCrossPlatform(TokenProvider tokenProvider) =>
            _tokenProvider = tokenProvider;

        public string? Authenticate(string login, string password)
        {
            try
            {
                // Подключение к LDAP-серверу
                using (var connection = new LdapConnection())
                {
                    connection.Connect(_ldapHost, _ldapPort);

                    // Аутентификация пользователя
                    string userDn = $"cn={login},dc=stud,dc=asu,dc=ru"; // DN пользователя
                    connection.Bind(userDn, password);

                    if (!connection.Bound)
                    {
                        Console.WriteLine("Authentication failed: Unable to bind to LDAP server.");
                        return null;
                    }

                    // Поиск пользователя в каталоге
                    string filter = $"(&(objectClass=user)(sAMAccountName={login}))";
                    string[] attributes = { "description", "cn" }; // Запрашиваемые атрибуты
                    var searchResults = connection.Search(
                        "dc=stud,dc=asu,dc=ru", // Базовый DN для поиска
                        LdapConnection.ScopeSub, // Область поиска
                        filter, // Фильтр поиска
                        attributes, // Атрибуты для получения
                        false // Не извлекать атрибуты оперативно
                    );

                    // Обработка результатов поиска
                    if (!searchResults.HasMore())
                    {
                        Console.WriteLine("User not found in LDAP directory.");
                        return null;
                    }

                    var searchResult = searchResults.Next();
                    var description = searchResult.GetAttribute("description")?.StringValue;
                    var fullName = searchResult.GetAttribute("cn")?.StringValue;

                    if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(fullName))
                    {
                        Console.WriteLine("Missing required attributes in LDAP entry.");
                        return null;
                    }

                    // Парсинг данных
                    var descriptionSplit = description.Split(',', 2, StringSplitOptions.None);
                    var groupTitle = descriptionSplit[0].Remove(0, 7); // Удаляем префикс "Group:"
                    var instituteTitle = descriptionSplit[1].Trim();

                    var fullNameSplit = fullName.Split(' ');
                    var lastName = fullNameSplit[0];
                    var firstName = fullNameSplit[1];
                    var middleName = fullNameSplit.Length > 2 ? fullNameSplit[2] : string.Empty;

                    // Создание объекта Student
                    var student = new Student(instituteTitle, groupTitle, firstName, lastName, middleName, login);

                    // Генерация токена
                    var token = _tokenProvider.Create(student);
                    return token;
                }
            }
            catch (LdapException ex)
            {
                Console.WriteLine($"LDAP error: {ex.Message} \n {ex.StackTrace}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message} \n {ex.StackTrace}");
                return null;
            }
        }
    }
}
