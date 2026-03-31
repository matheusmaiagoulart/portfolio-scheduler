using FluentResults;
using MediatR;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;

namespace PortfolioScheduler.Application.Commands.RegisterCustomerSubscriber;

public class RegisterCustomerSubscriberHandler : IRequestHandler<RegisterCustomerSubscriberCommand, Result<RegisterCustomerSubscriberResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IRecommendedPortfolioRepository _recommendedPortfolioRepository;
    public RegisterCustomerSubscriberHandler(ICustomerRepository customerRepository, IRecommendedPortfolioRepository recommendedPortfolioRepository)
    {
        _customerRepository = customerRepository;
        _recommendedPortfolioRepository = recommendedPortfolioRepository;
    }

    public async Task<Result<RegisterCustomerSubscriberResponse>> Handle(RegisterCustomerSubscriberCommand request, CancellationToken ct)
    {
        var customer = Customer.Create(request.Name, request.Cpf, request.Email, request.MonthlyAmount);
        if (customer.IsFailed)
            return Result.Fail(customer.Errors);

        var activePortfolio = await _recommendedPortfolioRepository.GetRecommendedPortfolioActive(ct);
        if (activePortfolio != null)
        {
            var result = customer.Value.BrokerageAccount.CreateInitialCustodies(activePortfolio.Items);
            if (result.IsFailed)
                return Result.Fail(result.Errors);
        }

        await _customerRepository.AddAsync(customer.Value, ct);

        var saveResult = await _customerRepository.SaveChangesAsync(ct);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);

        return Result.Ok(RegisterCustomerSubscriberResponse.SuccessRegister(customer.Value));
    }
}
