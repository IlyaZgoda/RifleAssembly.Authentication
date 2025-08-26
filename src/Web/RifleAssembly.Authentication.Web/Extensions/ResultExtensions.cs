using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RifleAssembly.Authentication.Web.Errors;
using RifleAssembly.WebService.SharedKernel.Result;

namespace RifleAssembly.Authentication.Web.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Value);
            }

            return result.Error.Code switch
            {
                ErrorTypes.InvalidCredentials => new UnauthorizedObjectResult(result.Error),
                ErrorTypes.NotFound => new NotFoundObjectResult(result.Error),
                _ => new BadRequestObjectResult(result.Error.Description)
            };
        }

        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return new OkResult();
            }

            return result.Error.Code switch
            {
                ErrorTypes.InvalidCredentials => new UnauthorizedObjectResult(result.Error),
                ErrorTypes.NotFound => new NotFoundObjectResult(result.Error),
                _ => new BadRequestObjectResult(result.Error.Description)
            };
        }

        public static async Task<IActionResult> ToActionResult<T>(this Task<Result<T>> resultTask)
        {
            var result = await resultTask;
            return result.ToActionResult();
        }

        public static async Task<IActionResult> ToActionResult(this Task<Result> resultTask)
        {
            var result = await resultTask;
            return result.ToActionResult();
        }
    }
}
