using FluentResults;
using MediatR;
using PortfolioScheduler.Application.Queries.Contracts;

namespace PortfolioScheduler.Application.Queries.GetCurrentRecommendedPortfolio;

public class GetCurrentRecommendedPortfolioHandler : IRequestHandler<GetCurrentRecommendedPortfolioQuery, Result<GetCurrentRecommendedPortfolioResponse>>
{
    private readonly IRecommendedPortfolioReadRepository _recommendedPortfolioReadRepository;

    public GetCurrentRecommendedPortfolioHandler(IRecommendedPortfolioReadRepository recommendedPortfolioReadRepository)
    {
        _recommendedPortfolioReadRepository = recommendedPortfolioReadRepository;
    }

    public async Task<Result<GetCurrentRecommendedPortfolioResponse>> Handle(GetCurrentRecommendedPortfolioQuery request, CancellationToken cancellationToken)
    {
        var recommendedPortfolio = await _recommendedPortfolioReadRepository.GetCurrentRecommendedPortfolioAsync(cancellationToken);
        if (recommendedPortfolio is null)
            return null;

        return Result.Ok(recommendedPortfolio);
    }
}
