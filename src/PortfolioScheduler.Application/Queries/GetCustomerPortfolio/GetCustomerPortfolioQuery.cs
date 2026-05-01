using FluentResults;
using MediatR;

namespace PortfolioScheduler.Application.Queries.GetCustomerPortfolio;

public record GetCustomerPortfolioQuery(long CustomerId) : IRequest<Result<GetCustomerPortfolioResponse>>;
