namespace PwcDotnet.Domain.AggregatesModel.RentalAggregate;

public class RentalPeriod : ValueObject
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public RentalPeriod(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new RentalDomainException("End date must be after start date");

        Start = start;
        End = end;
    }

    public bool OverlapsWith(RentalPeriod other)
    {
        return Start < other.End && other.Start < End;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}
