using RifleAssembly.WebService.SharedKernel.Result;

namespace RifleAssembly.WebService.SharedKernel.Result
{
    public partial class Result
    {
        public static async Task<Result<T>> EnsureExistOrCreate<T>(Func<Task<Result<T>>> getFunc, Func<Task<Result<T>>> createFunc)
        {
            var result = await getFunc();

            if (result.IsSuccess)
                return result;

            return await createFunc();
        }
    }
}
