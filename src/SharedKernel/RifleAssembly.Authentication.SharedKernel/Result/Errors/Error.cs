namespace RifleAssembly.WebService.SharedKernel.Result.Errors
{
    public record Error(string Code, string Description)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Error.NullValue", "Null value was provided");

        public static implicit operator string(Error error) => error?.Code ?? string.Empty;
    }
}
