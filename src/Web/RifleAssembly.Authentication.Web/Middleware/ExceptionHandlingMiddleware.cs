using Microsoft.AspNetCore.Diagnostics;
using RifleAssembly.Authentication.Web.Errors;
using RifleAssembly.Authentication.Web.Exceptions;
using RifleAssembly.Authentication.Web.Infrastructure.Factories;
using RifleAssembly.Authentication.Web.Mappers;
using RifleAssembly.WebService.SharedKernel.Result.Errors;

namespace RifleAssembly.Authentication.Web.Middleware
{
    public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger, ErrorToHttpMapper mapper, ProblemDetailsFactory problemDetailsFactory) 
        : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

            Error error = exception switch
            {
                LdapUnavailableException => LdapErrors.ServerUnavailable,
                _ => LdapErrors.InternalServerError,
            };

            var code = mapper.Map(error);

            httpContext.Response.StatusCode = code;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetailsFactory.CreateProblemDetails(error, code, httpContext)
            });
        }
    }
}
