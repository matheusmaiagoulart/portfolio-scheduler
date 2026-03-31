using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioScheduler.Api.Extensions;
using PortfolioScheduler.Application.Commands.CreateRecommendedPortfolio;

namespace PortfolioScheduler.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class PortfoliosController : ControllerBase
{

    private readonly IMediator _mediator;
    public PortfoliosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecommendedPortfolio(CreateRecommendedPortfolioCommand request)
    {
        var result = await _mediator.Send(request);
        return result.ValidateResult(this, 201);
    }
}
