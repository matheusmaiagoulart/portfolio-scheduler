using FluentResults;
using MediatR;
using PortfolioScheduler.Application.Services.DTOs;

namespace PortfolioScheduler.Application.Commands.RebalancePortfolio;

public record RebalancePortfolioCommand(PortfolioComparisonDTO PortfolioComparison) 
    : IRequest<Result<RebalancePortfolioResponse>>;
