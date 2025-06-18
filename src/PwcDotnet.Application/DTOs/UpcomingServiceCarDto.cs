namespace PwcDotnet.Application.DTOs;

public class UpcomingServiceCarDto
{
    public int CarId { get; init; }
    public string Model { get; init; } = "";
    public string Type { get; init; } = "";
    public DateTime ServiceDate { get; init; }
}
