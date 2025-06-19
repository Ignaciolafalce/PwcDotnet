namespace PwcDotnet.Application.DTOs;

public class RentalDto
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = default!;
    public int CarId { get; init; }
    public string CarModel { get; init; } = default!;
    public string CarType { get; init; } = default!;
    public int LocationId { get; init; }
    public string LocationName { get; init; } = default!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Status { get; init; } = default!;
}