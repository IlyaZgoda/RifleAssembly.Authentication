using Novell.Directory.Ldap;
using RifleAssembly.Authentication.Web.Errors;
using RifleAssembly.Authentication.Web.Exceptions;
using RifleAssembly.Authentication.Web.Students;
using RifleAssembly.WebService.SharedKernel.Result;

namespace RifleAssembly.Authentication.Web.Infrastructure.Services
{
    public class LdapCrossPlatform : ILdapService
    {
        private readonly TokenProvider _tokenProvider;
        private readonly ILogger<LdapCrossPlatform> _logger;
        private readonly string _ldapHost = "stud.asu.ru";
        private readonly int _ldapPort = 389; // Стандартный порт LDAP

        public LdapCrossPlatform(TokenProvider tokenProvider, ILogger<LdapCrossPlatform> logger)
        {
            _tokenProvider = tokenProvider;
            _logger = logger;
        }

        public async Task<Result<string>> AuthenticateAsync(string login, string password)
        {
            try
            {
                var result = await Task.Run(() =>
                {
                    // Подключение к LDAP-серверу
                    using var connection = new LdapConnection();
                    connection.Connect(_ldapHost, _ldapPort);
                    connection.Bind($"{login}@stud.asu.ru", password);

                    if (!connection.Bound)
                    {
                        _logger.LogInformation("Authentication failed: Unable to bind to LDAP server for user with login: {login}", login);
                        return Result.Failure<string>(LdapErrors.InvalidCredentials);
                    }

                    // Поиск пользователя в каталоге
                    string filter = $"(mail={login}@stud.asu.ru)";
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
                        _logger.LogInformation("User with login: {login} not found in LDAP directory", login);
                        return Result.Failure<string>(LdapErrors.NotFound);
                    }

                    var searchResult = searchResults.Next();
                    var description = searchResult.GetAttribute("description")?.StringValue;
                    var fullName = searchResult.GetAttribute("cn")?.StringValue;

                    if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(fullName))
                    {
                        _logger.LogInformation("Missing required attributes in LDAP entry for user with login: {login}", login);
                        return Result.Failure<string>(LdapErrors.NotFound);
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

                    _logger.LogInformation("User with login: {login} successfully authenticated", login);

                    return token;
                });
                return result;
            }
            catch (LdapException ex)
            {
                throw new LdapUnavailableException(LdapErrors.ServerUnavailable, ex);
            }
        }
    }
}