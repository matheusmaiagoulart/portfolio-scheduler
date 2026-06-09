using FluentResults;
using MediatR;
using PortfolioScheduler.Domain.DomainErrors;
using PortfolioScheduler.Domain.Repositories;

namespace PortfolioScheduler.Application.Commands.UpdateMontlhyAmount;

public class UpdateMonthlyAmountHandler : IRequestHandler<UpdateMonthlyAmountCommand, Result<UpdateMonthlyAmountResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public UpdateMonthlyAmountHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<UpdateMonthlyAmountResponse>> Handle(UpdateMonthlyAmountCommand request, CancellationToken ct)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, ct);
        if (customer == null)
            return Result.Fail(CustomerErrors.CustomerNotFound(request.CustomerId));

        var lastMonthlyAmount = customer.MonthlyAmount;

        var updateResult = customer.UpdateMonthlyAmount(request.Amount);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);

        var saveResult  = await _customerRepository.UpdateAsync(customer, ct);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);

        return Result.Ok(new UpdateMonthlyAmountResponse(customer.Id, lastMonthlyAmount, customer.MonthlyAmount));
    }
}
