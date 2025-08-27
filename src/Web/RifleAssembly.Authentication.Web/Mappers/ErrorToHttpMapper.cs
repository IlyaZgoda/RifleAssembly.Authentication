using RifleAssembly.Authentication.Web.Errors;
using RifleAssembly.WebService.SharedKernel.Result.Errors;

namespace RifleAssembly.Authentication.Web.Mappers
{
    public class ErrorToHttpMapper
    {
        public int Map(Error error) =>
            error.Code switch
            {
                ErrorTypes.InvalidCredentials => StatusCodes.Status401Unauthorized,
                ErrorTypes.NotFound => StatusCodes.Status404NotFound,
                ErrorTypes.LdapServerUnavailable => StatusCodes.Status503ServiceUnavailable,
                ErrorTypes.InternalServerError => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status400BadRequest,
            };
    }
}
