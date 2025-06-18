namespace PwcDotnet.Application.Validations;

public class CancelRentalValidator : AbstractValidator<CancelRentalCommand>
{
    public CancelRentalValidator()
    {
        RuleFor(x => x.RentalId).GreaterThan(0);
    }
}
