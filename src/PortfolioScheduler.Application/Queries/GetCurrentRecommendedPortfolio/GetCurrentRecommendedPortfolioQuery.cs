using FluentResults;
using MediatR;

namespace PortfolioScheduler.Application.Queries.GetCurrentRecommendedPortfolio;

public record GetCurrentRecommendedPortfolioQuery() : IRequest<Result<GetCurrentRecommendedPortfolioResponse>>;
