namespace PwcDotnet.Application.Interfaces;

public interface INotificationService 
{
    Task NotifyRentalCreatedAsync(int rentalId, int customerId, DateTime startDate, DateTime endDate);
}