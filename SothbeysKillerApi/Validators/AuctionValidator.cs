using FluentValidation;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Validators;

public class AuctionValidator : AbstractValidator<Auction>
{
    public AuctionValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty()
            .Length(3, 255);

        RuleFor(e => e.Start)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.Now);

        RuleFor(e => e.Finish)
            .NotEmpty()
            .GreaterThan(e => e.Start);
    }
}