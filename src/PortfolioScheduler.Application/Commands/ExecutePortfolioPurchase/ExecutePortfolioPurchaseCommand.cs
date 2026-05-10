using FluentResults;
using MediatR;
using PortfolioScheduler.Application.Behaviors;
using PortfolioScheduler.Domain.Services.DTOs;

namespace PortfolioScheduler.Application.Commands.ExecutePortfolioPurchase;

public record ExecutePortfolioPurchaseCommand(IEnumerable<QuoteDTO> LastCotahist) : IRequest<Result<ExecutePortfolioPurchaseResponse>>, ITransactionRequest;