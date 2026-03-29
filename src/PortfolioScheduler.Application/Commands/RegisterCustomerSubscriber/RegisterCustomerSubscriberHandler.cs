using FluentResults;
using MediatR;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;

namespace PortfolioScheduler.Application.Commands.RegisterCustomerSubscriber;

public class RegisterCustomerSubscriberHandler : IRequestHandler<RegisterCustomerSubscriberCommand, Result<RegisterCustomerSubscriberResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    public RegisterCustomerSubscriberHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<RegisterCustomerSubscriberResponse>> Handle(RegisterCustomerSubscriberCommand request, CancellationToken ct)
    {
        var customer = Customer.Create(request.Name, request.Cpf, request.Email, request.MonthlyAmount);
        if (customer.IsFailed)
            return Result.Fail(customer.Errors);

        await _customerRepository.AddAsync(customer.Value, ct);

        var saveResult = await _customerRepository.SaveChangesAsync(ct);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);

        return Result.Ok(RegisterCustomerSubscriberResponse.SuccessRegister(customer.Value));
    }
}
