using FluentValidation.Results;
using SothbeysKillerApi.Exceptions;

namespace SothbeysKillerApi.Extensions;

public static class ValidationExtensions
{
    public static void ThrowIfFail(this ValidationResult validationResult)
    {
        if (!validationResult.IsValid)
        {
            var errors = validationResult
                .Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));

            throw new AuctionValidationException(errors);
        }
    }
}