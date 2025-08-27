using Microsoft.AspNetCore.Mvc;
using RifleAssembly.WebService.SharedKernel.Result.Errors;

namespace RifleAssembly.Authentication.Web.Infrastructure.Factories
{
    public class ProblemDetailsFactory
    {
        public ProblemDetails CreateProblemDetails(Error error, int code, HttpContext httpContext)
        {
            return new ProblemDetails
            {
                Status = code,
                Title = error.Code,
                Detail = error.Description,
                Type = $"{GetTypeReference(code)}",
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["timestamp"] = DateTime.UtcNow,
                    ["requestId"] = httpContext.TraceIdentifier
                }
            };
        }

        private static string GetTypeReference(int statusCode) => 
            statusCode switch
        {
            // Client errors (4xx)
            400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            401 => "https://tools.ietf.org/html/rfc7235#section-3.1",
            402 => "https://tools.ietf.org/html/rfc7231#section-6.5.2",
            403 => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            405 => "https://tools.ietf.org/html/rfc7231#section-6.5.5",
            406 => "https://tools.ietf.org/html/rfc7231#section-6.5.6",
            408 => "https://tools.ietf.org/html/rfc7231#section-6.5.7",
            409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            410 => "https://tools.ietf.org/html/rfc7231#section-6.5.9",
            411 => "https://tools.ietf.org/html/rfc7231#section-6.5.10",
            412 => "https://tools.ietf.org/html/rfc7234#section-4.1",
            413 => "https://tools.ietf.org/html/rfc7231#section-6.5.11",
            414 => "https://tools.ietf.org/html/rfc7231#section-6.5.12",
            415 => "https://tools.ietf.org/html/rfc7231#section-6.5.13",
            416 => "https://tools.ietf.org/html/rfc7233#section-4.4",
            417 => "https://tools.ietf.org/html/rfc7231#section-6.5.14",
            422 => "https://tools.ietf.org/html/rfc4918#section-11.2",
            423 => "https://tools.ietf.org/html/rfc4918#section-11.3",
            424 => "https://tools.ietf.org/html/rfc4918#section-11.4",
            426 => "https://tools.ietf.org/html/rfc7231#section-6.5.15",
            428 => "https://tools.ietf.org/html/rfc6585#section-3",
            429 => "https://tools.ietf.org/html/rfc6585#section-4",
            431 => "https://tools.ietf.org/html/rfc6585#section-5",

            // Server errors (5xx)
            500 => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            501 => "https://tools.ietf.org/html/rfc7231#section-6.6.2",
            502 => "https://tools.ietf.org/html/rfc7231#section-6.6.3",
            503 => "https://tools.ietf.org/html/rfc7231#section-6.6.4",
            504 => "https://tools.ietf.org/html/rfc7231#section-6.6.5",
            505 => "https://tools.ietf.org/html/rfc7231#section-6.6.6",
            506 => "https://tools.ietf.org/html/rfc2295#section-8.1",
            507 => "https://tools.ietf.org/html/rfc4918#section-11.5",
            508 => "https://tools.ietf.org/html/rfc5842#section-7.2",
            510 => "https://tools.ietf.org/html/rfc2774#section-7",
            511 => "https://tools.ietf.org/html/rfc6585#section-6",

            _ => $"https://www.rfc-editor.org/rfc/rfc7231#section-6.{statusCode / 100}"
        };
    }
}
