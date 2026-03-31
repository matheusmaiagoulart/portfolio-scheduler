using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace PortfolioScheduler.Api.Extensions;

public static class ResultMapping
{
    public static IActionResult ValidateResult<T>(this Result<T> result, ControllerBase controller, int successStatusCode)
    {
        if (result.IsSuccess)
            return controller.StatusCode(successStatusCode, result.Value);

        var error = result.Errors.First();
        var statusCode = error.Metadata.TryGetValue("statusCode", out var code) ? (int)code : 500;

        return controller.StatusCode(statusCode, new { error = error.Message });
    }

    public static IActionResult ToActionResult(this Result result, ControllerBase controller, int successStatusCode = 200)
    {
        if (result.IsSuccess)
            return controller.StatusCode(successStatusCode);

        var error = result.Errors.First();
        var statusCode = error.Metadata.TryGetValue("statusCode", out var code) ? (int)code : 500;

        return controller.StatusCode(statusCode, new { error = error.Message });
    }
}
