namespace PwcDotnet.Application.DTOs;

public class TopCarGroupDto
{
    public int LocationId { get; init; }
    public string LocationName { get; init; } = "";
    public string Brand { get; init; } = "";
    public string Model { get; init; } = "";
    public string Type { get; init; } = "";
    public int TotalRentals { get; init; }
}