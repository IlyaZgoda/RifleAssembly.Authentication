using RifleAssembly.WebService.SharedKernel.Result.Errors;

namespace RifleAssembly.WebService.SharedKernel.Result
{
    public partial class Result<TValue> : Result
    {
        private readonly TValue _value = default!;

        public TValue Value =>
            IsSuccess ?
            _value :
            throw new InvalidOperationException("The value of a failure result can not be accessed.");

        protected internal Result(TValue value, bool isSuccess, Error error)
            : base(isSuccess, error) =>
            _value = value;

        public static implicit operator Result<TValue>(TValue? value) =>
            value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    }
}
