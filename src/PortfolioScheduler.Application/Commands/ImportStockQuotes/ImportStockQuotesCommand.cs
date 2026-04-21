using FluentResults;
using MediatR;
using PortfolioScheduler.Domain.Services.DTOs;

namespace PortfolioScheduler.Application.Commands.ImportStockQuotes;

public record ImportStockQuotesCommand(DateOnly ReferenceDate) : IRequest<Result<IEnumerable<QuoteDTO>>>;
