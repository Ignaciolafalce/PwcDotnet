namespace PwcDotnet.Application.DTOs;

public class DailyStatDto
{
    public DateTime Date { get; init; }
    public int Rentals { get; init; }
    public int Cancellations { get; init; }
    public int UnusedCars { get; init; }
}