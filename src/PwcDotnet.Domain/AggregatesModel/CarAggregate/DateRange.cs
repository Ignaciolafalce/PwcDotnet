namespace PwcDotnet.Domain.AggregatesModel.CarAggregate;

public class DateRange : ValueObject
{
    public DateTime From { get; }
    public DateTime To { get; }

    public DateRange(DateTime from, DateTime to)
    {
        if (to < from)
            throw new RentalDomainException("Invalid date range");

        From = from.Date;
        To = to.Date;
    }

    public bool Contains(DateTime date)
    {
        return date.Date >= From && date.Date <= To;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return From;
        yield return To;
    }
}