using RifleAssembly.WebService.SharedKernel.Result.Errors;
using System;

namespace RifleAssembly.WebService.SharedKernel.Result
{
    public partial class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        protected internal Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None ||
                !isSuccess && error == Error.None)

                throw new ArgumentException("Invalid error", nameof(error));

            IsSuccess = isSuccess;
            Error = error;
        }
    }
}
