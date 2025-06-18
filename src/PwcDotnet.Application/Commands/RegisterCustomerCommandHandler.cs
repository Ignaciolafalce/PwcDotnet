namespace PwcDotnet.Application.Commands;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, int>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(request.Street, request.City, request.Country);
        var customer = new Customer(request.FullName, address, request.Email);

        await _customerRepository.AddAsync(customer);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return customer.Id;
    }
}