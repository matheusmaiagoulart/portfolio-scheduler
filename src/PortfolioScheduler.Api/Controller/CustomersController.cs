using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioScheduler.Api.Extensions;
using PortfolioScheduler.Application.Commands.RegisterCustomerSubscriber;
using PortfolioScheduler.Application.Queries.GetCustomerPortfolio;

namespace PortfolioScheduler.Api.Controller;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("adesao")]
    public async Task<IActionResult> RegisterCustomerSubscriber(RegisterCustomerSubscriberCommand request)
    {
        var result = await _mediator.Send(request);
        return result.ValidateResult(this, 201);
    }

    [HttpGet("{customerId}/carteira")]
    public async Task<IActionResult> GetCustomerPortfolio(long customerId)
    {
        var request = new GetCustomerPortfolioQuery(customerId);
        var result = await _mediator.Send(request);
        return result.ValidateResult(this, 200);
    }
}
