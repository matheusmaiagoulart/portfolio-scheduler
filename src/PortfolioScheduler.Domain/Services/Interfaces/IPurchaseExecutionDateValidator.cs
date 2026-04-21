using FluentResults;

namespace PortfolioScheduler.Domain.Services.Interfaces;

public interface IPurchaseExecutionDateValidator
{
    Result ValidatePurchaseDay(DateTime date);
}
