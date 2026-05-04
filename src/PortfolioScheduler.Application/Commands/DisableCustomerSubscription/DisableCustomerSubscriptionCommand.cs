using FluentResults;
using MediatR;

namespace PortfolioScheduler.Application.Commands.DisableCustomerSubscription;

public record DisableCustomerSubscriptionCommand(long customerId) : IRequest<Result<DisableCustomerSubscriptionResponse>>;
