using FluentResults;
using MediatR;
using PortfolioScheduler.Application.Queries.Contracts;
using PortfolioScheduler.Domain.DomainErrors;

namespace PortfolioScheduler.Application.Queries.GetAllRecommendedPortfolio;

public class GetAllRecommendedPortfolioHandler : IRequestHandler<GetAllRecommendedPortfolioQuery, Result<GetAllRecommendedPortfolioResponse>>
{
    private readonly IRecommendedPortfolioReadRepository _portfolioReadRepository;

    public GetAllRecommendedPortfolioHandler(IRecommendedPortfolioReadRepository portfolioReadRepository)
    {
        _portfolioReadRepository = portfolioReadRepository;
    }

    public async Task<Result<GetAllRecommendedPortfolioResponse>> Handle(GetAllRecommendedPortfolioQuery request, CancellationToken ct)
    {
        var portfolios = await _portfolioReadRepository.GetAllRecommendedPortfolioAsync(ct);
        if (!portfolios.portfolios.Any()) {
            return Result.Fail(PortfolioErrors.NoOneRecommendedPortfoliosRegistered());
        }
        return Result.Ok(portfolios);
    }
}
