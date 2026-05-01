using FluentResults;
using MediatR;
using PortfolioScheduler.Domain.DomainErrors;
using PortfolioScheduler.Application.Queries.Contracts;


namespace PortfolioScheduler.Application.Queries.GetCustomerPortfolio;

public class GetCustomerPortfolioHandler : IRequestHandler<GetCustomerPortfolioQuery, Result<GetCustomerPortfolioResponse>>
{
    private readonly ICustomerReadRepository _customerReadRepository;

    public GetCustomerPortfolioHandler(ICustomerReadRepository customerReadRepository)
    {
        _customerReadRepository = customerReadRepository;
    }

    public async Task<Result<GetCustomerPortfolioResponse>> Handle(GetCustomerPortfolioQuery request, CancellationToken ct)
    {
        var response =  await _customerReadRepository.GetCustomerPortfolio(request.CustomerId, ct);
        if (response == null)
            return Result.Fail<GetCustomerPortfolioResponse>(CustomerErrors.CustomerNotFound(request.CustomerId));

        return Result.Ok(response);
    }
}
