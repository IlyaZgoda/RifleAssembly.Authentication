using RifleAssembly.WebService.SharedKernel.Result.Errors;

namespace RifleAssembly.Authentication.Web.Exceptions
{
    public class LdapUnavailableException : Exception
    {
        public Error Error { get; }
        public Exception Inner { get; }
        public LdapUnavailableException(Error error, Exception inner) : base(error.Description, inner)
        {
            Error = error;
            Inner = inner;
        }
    }
}
