using FluentResults;
using MediatR;
using PortfolioScheduler.Application.Behaviors;

namespace PortfolioScheduler.Application.Commands.RegisterCustomerSubscriber;

public record RegisterCustomerSubscriberCommand(
    string Name,
    string Cpf,
    string Email,
    decimal MonthlyAmount) : IRequest<Result<RegisterCustomerSubscriberResponse>>, ITransactionRequest;
