using RifleAssembly.WebService.SharedKernel.Result;

namespace RifleAssembly.WebService.SharedKernel.Result.Methods.Extensions
{
    public static partial class ResultExtensions
    {
        public static async Task<Result> Bind<TIn>(this Result<TIn> result, Func<TIn, Task<Result>> func) =>
            result.IsSuccess ? await func(result.Value) : Result.Failure(result.Error);

        public static async Task<Result<TOut>> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> func) 
            where TOut: class =>
            result.IsSuccess? await func(result.Value) : Result.Failure<TOut>(result.Error);

        public static async Task<Result<T>> Bind<T>(this Result result, Func<Task<Result<T>>> func)
        {
            if (result.IsFailure) return Result.Failure<T>(result.Error);
            return await func();
        }

        public static async Task<Result> Bind(this Result result, Func<Task<Result>> func)
        {
            if (result.IsFailure) return result;
            return await func();
        }
    }
}
