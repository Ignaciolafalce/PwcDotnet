using PwcDotnet.Domain.AggregatesModel.CarAggregate;
using System.Runtime.Versioning;

namespace PwcDotnet.Application.Queries;

public class CheckCarAvailabilityQueryHandler : IRequestHandler<CheckCarAvailabilityQuery, List<AvailableCarDto>>
{
    private readonly ICarRepository _carRepository;
    private readonly IRentalRepository _rentalRepository;

    public CheckCarAvailabilityQueryHandler(ICarRepository carRepository, IRentalRepository rentalRepository)
    {
        _carRepository = carRepository;
        _rentalRepository = rentalRepository;
    }

    public async Task<List<AvailableCarDto>> Handle(CheckCarAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var range = new DateRange(request.StartDate, request.EndDate);
        CarType? filterType = null;

        if (!string.IsNullOrWhiteSpace(request.CarType))
        {
            filterType = new CarType(request.CarType);
        }

        var cars = await _carRepository.GetAvailableOfServicesCarsAsync(range, filterType);
        var reservedCarsIds = (await _rentalRepository.GetRentalsByDateRangeAsync(range.From, range.To))
                                                   .Select(r => r.CarId)
                                                   .ToHashSet();

        var availableNotReservedCars = cars.Where(c => !reservedCarsIds.Contains(c.Id)).ToList();

        var availableCarsDto = availableNotReservedCars
                                .Select(c => new AvailableCarDto
                                {
                                    Id = c.Id,
                                    LocationId = c.LocationId,
                                    LocationName = c.Location.Name,
                                    Model = c.Model,
                                    Type = c.Type.Name
                                }).ToList();

        return availableCarsDto;
    }
}