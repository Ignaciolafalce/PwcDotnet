namespace PwcDotnet.Application.DTOs;

public class AvailableCarDto
{
    public int Id { get; init; }
    public int LocationId { get; init; }
    public string LocationName { get; init; } = "";
    public string Model { get; init; } = "";
    public string Type { get; init; } = "";
}
