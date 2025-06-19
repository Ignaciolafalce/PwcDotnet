namespace PwcDotnet.Application.Queries;

public record GetAllCustomersQuery() : IRequest<List<CustomerDto>>;