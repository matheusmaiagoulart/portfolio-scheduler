using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioScheduler.Domain.Repositories;

namespace PortfolioScheduler.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IAppDbContext _dbContext;
    public TransactionBehavior(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ITransactionRequest)
        {
            return await next(cancellationToken);
        }

        try
        {
            await _dbContext.BeginTransactionAsync(cancellationToken);

            var response = await next(cancellationToken);
            if (response.ToResult().IsFailed)
            {
                await _dbContext.RollbackAsync(cancellationToken);
                return response;
            }

            await _dbContext.CommitAsync(cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            await _dbContext.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
