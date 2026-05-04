using FluentResults;
using MediatR;

namespace PortfolioScheduler.Application.Queries.GetAllRecommendedPortfolio
{
    public record GetAllRecommendedPortfolioQuery() : IRequest<Result<GetAllRecommendedPortfolioResponse>>;
}
