using FluentResults;
using PortfolioScheduler.Domain.Services.Interfaces;

namespace PortfolioScheduler.Domain.Services.Implementations;

public class PurchaseExecutionDateValidator : IPurchaseExecutionDateValidator
{
    public Result ValidatePurchaseDay(DateTime date)
    {
        var executionDayResult = ValidateExecutionDay(date);
        if (executionDayResult.IsFailed)
        {
            return executionDayResult;
        }

        var businessDayResult = ValidateBusinessDay(date);
        if (businessDayResult.IsFailed)
        {
            return businessDayResult;
        }

        return Result.Ok();
    }

    private Result ValidateExecutionDay(DateTime date)
    {
        if (date.Day != 5 && date.Day != 15 && date.Day != 25)
        {
            return Result.Fail(new Error("A data informada não é um dia disponível para execução da compra.")
                .WithMetadata("statusCode", 400));
        }

        return Result.Ok();
    }

    private Result ValidateBusinessDay(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            var nextBusinessDay = date.AddDays(date.DayOfWeek == DayOfWeek.Saturday ? 2 : 1);
            return Result.Fail(new Error($"A data informada é um dia não útil. " +
                $"O próximo dia útil é {nextBusinessDay:dd/MM/yyyy}.").WithMetadata("statusCode", 400));
        }

        return Result.Ok();
    }
}
