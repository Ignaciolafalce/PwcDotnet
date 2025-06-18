namespace PwcDotnet.Application.DTOs;

public class TopCarGroupDto
{
    public string Brand { get; init; } = "";
    public string Model { get; init; } = "";
    public string Type { get; init; } = "";
    public int TotalRentals { get; init; }
}