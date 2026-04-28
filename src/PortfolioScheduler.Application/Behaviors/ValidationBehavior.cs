using FluentValidation;
using MediatR;

namespace PortfolioScheduler.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehavior(IValidator<TRequest>? validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validator == null) // For handlers that don't have a body, we just skip validation
            return await next();

        var result = await _validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        return await next();
    }
}