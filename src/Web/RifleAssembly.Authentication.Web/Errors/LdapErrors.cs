using RifleAssembly.WebService.SharedKernel.Result.Errors;

namespace RifleAssembly.Authentication.Web.Errors
{
    public static class LdapErrors
    {
        public static Error InvalidCredentials => new(ErrorTypes.InvalidCredentials, "Invalid credentials");
        public static Error NotFound => new(ErrorTypes.NotFound, "User not found in LDAP directory");
        public static Error ServerUnavailable => new(ErrorTypes.LdapServerUnavailable, "Ldap server unavailable");
        public static Error InternalServerError => new(ErrorTypes.InternalServerError, "Internal server error");
    }
}
