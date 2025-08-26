using RifleAssembly.WebService.SharedKernel.Result;
using RifleAssembly.WebService.SharedKernel.Result.Errors;

namespace RifleAssembly.WebService.SharedKernel.Result
{
    public partial class Result
    {
        public static Result<TValue> Create<TValue>(TValue? value, Error error) 
            where TValue : class =>
            value is null ? Failure<TValue>(error) : Success(value);
    }
}
