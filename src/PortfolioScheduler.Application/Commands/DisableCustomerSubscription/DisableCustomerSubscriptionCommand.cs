using FluentResults;
using MediatR;
using PortfolioScheduler.Application.Behaviors;

namespace PortfolioScheduler.Application.Commands.DisableCustomerSubscription;

public record DisableCustomerSubscriptionCommand(long customerId) : IRequest<Result<DisableCustomerSubscriptionResponse>>, ITransactionRequest;
