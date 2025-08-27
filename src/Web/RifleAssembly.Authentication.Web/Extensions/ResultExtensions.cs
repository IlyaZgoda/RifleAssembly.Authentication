using Microsoft.AspNetCore.Mvc;
using RifleAssembly.Authentication.Web.Infrastructure.Factories;
using RifleAssembly.Authentication.Web.Mappers;
using RifleAssembly.WebService.SharedKernel.Result;

namespace RifleAssembly.Authentication.Web.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result, 
            HttpContext httpContext, 
            ErrorToHttpMapper mapper, 
            ProblemDetailsFactory problemDetailsFactory)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Value);
            }

            var statusCode = mapper.Map(result.Error);

            var problemDetails = problemDetailsFactory.CreateProblemDetails(result.Error, statusCode, httpContext);

            return new ObjectResult(problemDetails);
        }

        public static IActionResult ToActionResult(this Result result, 
            HttpContext httpContext, 
            ErrorToHttpMapper mapper, 
            ProblemDetailsFactory problemDetailsFactory)
        {
            if (result.IsSuccess)
            {
                return new OkResult();
            }
            var statusCode = mapper.Map(result.Error);
            var problemDetails = problemDetailsFactory.CreateProblemDetails(result.Error, statusCode, httpContext);

            return new ObjectResult(problemDetails);
        }

        public static async Task<IActionResult> ToActionResult<T>(this Task<Result<T>> resultTask, 
            HttpContext httpContext, 
            ErrorToHttpMapper mapper, 
            ProblemDetailsFactory problemDetailsFactory)
        {
            var result = await resultTask;
            return result.ToActionResult(httpContext, mapper, problemDetailsFactory);
        }

        public static async Task<IActionResult> ToActionResult(this Task<Result> resultTask, HttpContext httpContext, ErrorToHttpMapper mapper, ProblemDetailsFactory problemDetailsFactory)
        {
            var result = await resultTask;
            return result.ToActionResult(httpContext, mapper, problemDetailsFactory);
        }
    }
}
