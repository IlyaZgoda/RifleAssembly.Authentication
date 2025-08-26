using RifleAssembly.WebService.SharedKernel.Result.Errors;

namespace RifleAssembly.WebService.SharedKernel.Result
{
    public partial class Result
    {
        public static Result Success() =>
            new(true, Error.None);

        public static Result<TValue> Success<TValue>(TValue value) =>
            new(value, true, Error.None);

    }
}
