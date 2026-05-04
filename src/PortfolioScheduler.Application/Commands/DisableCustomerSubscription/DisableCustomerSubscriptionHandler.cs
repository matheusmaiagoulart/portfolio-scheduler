using FluentResults;
using MediatR;
using PortfolioScheduler.Domain.DomainErrors;
using PortfolioScheduler.Domain.Repositories;

namespace PortfolioScheduler.Application.Commands.DisableCustomerSubscription;

public class DisableCustomerSubscriptionHandler : IRequestHandler<DisableCustomerSubscriptionCommand, Result<DisableCustomerSubscriptionResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public DisableCustomerSubscriptionHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<DisableCustomerSubscriptionResponse>> Handle(DisableCustomerSubscriptionCommand request, CancellationToken ct)
    {
        var customer = await _customerRepository.GetByIdAsync(request.customerId, ct);
        if (customer == null)
            return Result.Fail(CustomerErrors.CustomerNotFound(request.customerId));

        var resultDisable = customer.Disable();
        if (resultDisable.IsFailed)
            return Result.Fail(resultDisable.Errors);

        var resultUpdate = await _customerRepository.UpdateAsync(customer, ct);
        if (resultUpdate.IsFailed)
            return Result.Fail(resultUpdate.Errors);

        return Result.Ok(new DisableCustomerSubscriptionResponse
        (
            customerId: customer.Id,
            name: customer.Name,
            active: customer.Active,
            exitDate: DateTime.UtcNow
        ));
    }
}
