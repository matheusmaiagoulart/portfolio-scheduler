using FluentResults;
using MediatR;
using PortfolioScheduler.Application.Behaviors;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Application.Commands.CreateRecommendedPortfolio;

public record CreateRecommendedPortfolioCommand(
    string Name,
    List<PortfolioItem> PortfolioItems,
    DateTime? TerminationDate) : IRequest<Result<CreateRecommendedPortfolioResponse>>, ITransactionRequest;
