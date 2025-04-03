namespace SothbeysKillerApi.Exceptions;

public class AuctionValidationException : Exception
{
    public string Field { get; }
    public string Description { get; }

    public AuctionValidationException(string field, string description)
    {
        Field = field;
        Description = description;
    }
}