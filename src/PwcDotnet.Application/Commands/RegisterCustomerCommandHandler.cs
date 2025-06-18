using PwcDotnet.Application.Interfaces;

namespace PwcDotnet.Application.Commands;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, int>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUser;

    public RegisterCustomerCommandHandler(ICustomerRepository customerRepository, ICurrentUserService currentUser)
    {
        _customerRepository = customerRepository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(request.Street, request.City, request.Country);
        var customer = new Customer(request.FullName, address, request.Email);
        customer.LinkToUser(_currentUser.UserId!);

        await _customerRepository.AddAsync(customer);
        await _customerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return customer.Id;
    }
}