using MediatR;
using Microsoft.AspNetCore.Mvc;
using PortfolioScheduler.Api.Extensions;
using PortfolioScheduler.Application.Commands.RegisterCustomerSubscriber;

namespace PortfolioScheduler.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterCustomerSubscriber(RegisterCustomerSubscriberCommand request)
    {
        var result = await _mediator.Send(request);
        return result.ValidateResult(this);
    }
}
