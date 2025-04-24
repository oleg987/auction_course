namespace SothbeysKillerApi.Exceptions;

public class AuctionValidationException : Exception
{
    public IEnumerable<ValidationError> Errors { get; }
    public AuctionValidationException(IEnumerable<ValidationError> errors)
    {
        Errors = errors;
    }
}

public record ValidationError(string Field, string Description);