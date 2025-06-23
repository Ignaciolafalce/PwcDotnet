namespace PwcDotnet.Infrastructure.Services
{
    public record SendRentalEmailDto(int RentalId, string Email, DateTime StartDate, DateTime EndDate);

}
