namespace PwcDotnet.Application.DTOs;

public class CustomerDto
{
    public int Id { get; init; }
    public string FullName { get; init; } = default!;
    public string Email { get; init; } = default!;
}
