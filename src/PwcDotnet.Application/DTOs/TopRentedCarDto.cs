namespace PwcDotnet.Application.DTOs;

public class TopRentedCarDto
{
    public int CarId { get; init; }
    public string Model { get; init; } = "";
    public string Type { get; init; } = "";
    public int TotalRentals { get; init; }
}