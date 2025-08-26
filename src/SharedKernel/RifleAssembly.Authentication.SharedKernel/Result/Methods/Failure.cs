using RifleAssembly.WebService.SharedKernel.Result;
using RifleAssembly.WebService.SharedKernel.Result.Errors;

namespace RifleAssembly.WebService.SharedKernel.Result
{
    public partial class Result
    {
        public static Result Failure(Error error) =>
            new(false, error);

        public static Result<TValue> Failure<TValue>(Error error) =>
            new(default!, false, error);
    }
}
