namespace PwcDotnet.Domain.AggregatesModel.CarAggregate;

public class Service : Entity
{
    public DateTime Date { get; private set; }

    protected Service() { }

    public Service(DateTime date)
    {
        Date = date;
    }
}
