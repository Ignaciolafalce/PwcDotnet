namespace PwcDotnet.Application.Validations;

public class RegisterRentalValidator : AbstractValidator<RegisterRentalCommand>
{
    public RegisterRentalValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.CarId).GreaterThan(0);
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("Start date must be before end date");
        RuleFor(x => x.StartDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date must be today or in the future");
    }
}