using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioScheduler.Api.Extensions;
using PortfolioScheduler.Application.Commands.CreateRecommendedPortfolio;
using PortfolioScheduler.Application.Queries.GetCurrentRecommendedPortfolio;

namespace PortfolioScheduler.Api.Controller;

[ApiController]
[Route("api/admin/cesta")]
public class PortfoliosController : ControllerBase
{

    private readonly IMediator _mediator;
    public PortfoliosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost()]
    public async Task<IActionResult> CreateRecommendedPortfolio(CreateRecommendedPortfolioCommand request)
    {
        var result = await _mediator.Send(request);
        return result.ValidateResult(this, 201);
    }

    [HttpGet("atual")]
    public async Task<IActionResult> GetCurrentRecommendedPortfolio()
    {
        var result = await _mediator.Send(new GetCurrentRecommendedPortfolioQuery());
        return result.ValidateResult(this, 200);
    }
}
