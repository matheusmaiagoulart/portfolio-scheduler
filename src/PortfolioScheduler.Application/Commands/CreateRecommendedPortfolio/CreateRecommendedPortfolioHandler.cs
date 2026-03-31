using FluentResults;
using MediatR;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;

namespace PortfolioScheduler.Application.Commands.CreateRecommendedPortfolio;

public class CreateRecommendedPortfolioHandler : IRequestHandler<CreateRecommendedPortfolioCommand, Result<CreateRecommendedPortfolioResponse>>
{
    private readonly IRecommendedPortfolioRepository _portfolioRepository;

    public CreateRecommendedPortfolioHandler(IRecommendedPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
    }

    public async Task<Result<CreateRecommendedPortfolioResponse>> Handle(CreateRecommendedPortfolioCommand request, CancellationToken ct)
    {
        var portfolio = RecommendedPortfolio.Create(request.Name, request.PortfolioItems, request.TerminationDate);
        if (portfolio.IsFailed)
            return Result.Fail<CreateRecommendedPortfolioResponse>(portfolio.Errors);

        await _portfolioRepository.AddAsync(portfolio.Value, ct);

        var saveResult = await _portfolioRepository.SaveChangesAsync(portfolio.Value.Id, ct);
        if (saveResult.IsFailed)
            return Result.Fail<CreateRecommendedPortfolioResponse>(saveResult.Errors);

        return Result.Ok(CreateRecommendedPortfolioResponse.SuccessfullyCreated(portfolio.Value));
    }
}
