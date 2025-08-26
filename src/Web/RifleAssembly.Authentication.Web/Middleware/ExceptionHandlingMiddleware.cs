using RifleAssembly.Authentication.Web.Contracts;
using RifleAssembly.Authentication.Web.Errors;
using RifleAssembly.Authentication.Web.Exceptions;
using RifleAssembly.WebService.SharedKernel.Result.Errors;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace RifleAssembly.Authentication.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred: {Message}", ex.Message);

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            (HttpStatusCode httpStatusCode, IReadOnlyCollection<Error> errors) = GetHttpStatusCodeAndErrors(exception);

            httpContext.Response.ContentType = "application/json";

            httpContext.Response.StatusCode = (int)httpStatusCode;

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string response = JsonSerializer.Serialize(new ApiErrorResponse(errors), serializerOptions);

            await httpContext.Response.WriteAsync(response);
        }

        private static (HttpStatusCode httpStatusCode, IReadOnlyCollection<Error>) GetHttpStatusCodeAndErrors(Exception exception) =>
            exception switch
            {
                LdapUnavailableException ldapUnavailableException => (HttpStatusCode.ServiceUnavailable, new[] { LdapErrors.ServerUnavailable }),
                _ => (HttpStatusCode.InternalServerError, new[] { LdapErrors.InternalServerError })
            };
    }
}
