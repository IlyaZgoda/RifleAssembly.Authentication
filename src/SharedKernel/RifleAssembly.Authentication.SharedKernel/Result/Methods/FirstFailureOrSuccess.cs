namespace RifleAssembly.WebService.SharedKernel.Result
{
    public partial class Result
    {
        public static Result FirstFailureOrSuccess(params Result[] results)
        {
            foreach (Result result in results)
                if (result.IsFailure)
                    return result;
                
            return Success();
        }
    }
}
