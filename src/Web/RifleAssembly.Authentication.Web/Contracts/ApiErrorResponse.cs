using RifleAssembly.WebService.SharedKernel.Result.Errors;

namespace RifleAssembly.Authentication.Web.Contracts
{
    public class ApiErrorResponse
    {
        public ApiErrorResponse(IReadOnlyCollection<Error> errors) => Errors = errors;

        public IReadOnlyCollection<Error> Errors { get; }

    }
}
