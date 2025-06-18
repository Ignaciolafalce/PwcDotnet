namespace PwcDotnet.Application.Validations;

public class ModifyRentalValidator : AbstractValidator<ModifyRentalCommand>
{
    public ModifyRentalValidator()
    {
        RuleFor(x => x.RentalId).GreaterThan(0);
        RuleFor(x => x.NewStartDate).LessThan(x => x.NewEndDate);
        RuleFor(x => x.NewStartDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date);
    }
}
