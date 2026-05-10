using MediatR;

namespace PortfolioScheduler.Application.Behaviors;

/// <summary>
/// Marker interface to indicate that a request should be handled within a transaction. 
/// For Queries requests, the transaction will not be initiated and the TransactionBehavior will simply pass the request to the next handler in the pipeline.
/// </summary>
public interface ITransactionRequest
{ 
}
