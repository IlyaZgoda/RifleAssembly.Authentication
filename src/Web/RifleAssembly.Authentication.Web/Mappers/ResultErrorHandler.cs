using Microsoft.AspNetCore.Mvc;
using RifleAssembly.WebService.SharedKernel.Result;
using RifleAssembly.WebService.SharedKernel.Result.Errors;
using ProblemDetailsFactory = RifleAssembly.Authentication.Web.Infrastructure.Factories.ProblemDetailsFactory;

namespace RifleAssembly.Authentication.Web.Mappers
{
    public class ResultErrorHandler
    {
        private readonly ErrorToHttpMapper _errorToHttpMapper;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public ResultErrorHandler(ErrorToHttpMapper errorToHttpMapper, ProblemDetailsFactory problemDetailsFactory)
        {
            _errorToHttpMapper = errorToHttpMapper;
            _problemDetailsFactory = problemDetailsFactory;
        }

        public IActionResult Handle(Error error, HttpContext context)
        {
            var statusCode = _errorToHttpMapper.Map(error);
            var problemDetails = _problemDetailsFactory.CreateProblemDetails(error, statusCode, context);

            return new ObjectResult(problemDetails);
        }
    }
}
