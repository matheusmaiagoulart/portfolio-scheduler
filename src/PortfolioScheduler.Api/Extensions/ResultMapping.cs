using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioScheduler.Api.Extensions;

public static class ResultMapping
{
    public static IActionResult ValidateResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        var errors = new { errors = result.Errors.Select(e => e.Message) };

        var firstErrorMessage = result.Errors.First().Message;

        if (firstErrorMessage.Contains("CPF", StringComparison.OrdinalIgnoreCase) ||
            firstErrorMessage.Contains("Email", StringComparison.OrdinalIgnoreCase))
        {
            return controller.Conflict(errors);
        }

        return controller.StatusCode(500, errors);
    }
}
