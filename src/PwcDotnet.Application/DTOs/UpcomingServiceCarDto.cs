namespace PwcDotnet.Application.DTOs;

public class UpcomingServiceCarDto
{
    public int CarId { get; init; }
    public int LocationId { get; init; }
    public string LocationName { get; init; } = "";
    public string Model { get; init; } = "";
    public string Type { get; init; } = "";
    public DateTime ServiceDate { get; init; }
}
