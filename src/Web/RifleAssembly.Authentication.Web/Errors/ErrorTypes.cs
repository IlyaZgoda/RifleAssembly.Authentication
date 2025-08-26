namespace RifleAssembly.Authentication.Web.Errors
{
    public static class ErrorTypes
    {
        public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";
        public const string NotFound = "NOT_FOUND";
        public const string LdapServerUnavailable = "LDAP_SERVER_UNAVAILABLE";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    }
}
