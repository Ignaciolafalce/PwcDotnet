using Microsoft.Extensions.Caching.Memory;
using PwcDotnet.Domain.Common.SeedWork;

namespace PwcDotnet.Application.Queries;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, List<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMemoryCache _cache;

    public GetAllCustomersQueryHandler(ICustomerRepository customerRepository, IMemoryCache cache)
    {
        _customerRepository = customerRepository;
        _cache = cache;
    }

    public async Task<List<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customerDtoList =  await _cache.GetOrCreateAsync("customers_cache", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3);

            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email
            }).ToList();
        });

        return customerDtoList ?? new List<CustomerDto>();
    }
}