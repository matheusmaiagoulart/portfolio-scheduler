using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioScheduler.Api.Extensions;
using PortfolioScheduler.Application.Commands.ExecutePortfolioPurchase;
using PortfolioScheduler.Application.Commands.ImportStockQuotes;

namespace PortfolioScheduler.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class PurchaseController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchaseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("execute-purchase")]
    public async Task<IActionResult> ExecutePurchase(ImportStockQuotesCommand request)
    {
        var result = await _mediator.Send(request);
        if (!result.IsSuccess)
            return result.ValidateResult(this, 200);

        var purchaseResult = await _mediator.Send(new ExecutePortfolioPurchaseCommand(result.Value));
        if (!purchaseResult.IsSuccess)
            return purchaseResult.ValidateResult(this, 200);

        return purchaseResult.ValidateResult(this, 201);
    }
}
