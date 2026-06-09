using FluentResults;
using MediatR;
using PortfolioScheduler.Application.Behaviors;

namespace PortfolioScheduler.Application.Commands.UpdateMontlhyAmount;

public record UpdateMonthlyAmountCommand(
    long CustomerId, 
    decimal Amount) : IRequest<Result<UpdateMonthlyAmountResponse>>, ITransactionRequest;
