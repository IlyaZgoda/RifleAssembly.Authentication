namespace RifleAssembly.WebService.SharedKernel.Result.Methods.Extensions
{
    public static partial class ResultExtensions
    {
        public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> predicate)
            where TOut : class =>
            result.IsSuccess ? predicate(result.Value) : Result.Failure<TOut>(result.Error);

        public static Result<TOut?> MapNullable<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
            where TOut : class =>
            result.IsSuccess ? Result.Success<TOut?>(map(result.Value)) : Result.Failure<TOut?>(result.Error);
    }
}
