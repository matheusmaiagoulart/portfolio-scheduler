using FluentResults;
using MediatR;
using PortfolioScheduler.Application.Commands.RebalancePortfolio;
using PortfolioScheduler.Application.Services.DTOs;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;

namespace PortfolioScheduler.Application.Commands.CreateRecommendedPortfolio;

public class CreateRecommendedPortfolioHandler : IRequestHandler<CreateRecommendedPortfolioCommand, Result<CreateRecommendedPortfolioResponse>>
{
    private readonly IMediator _mediator;
    private readonly IRecommendedPortfolioRepository _portfolioRepository;

    public CreateRecommendedPortfolioHandler(IMediator mediator, IRecommendedPortfolioRepository portfolioRepository)
    {
        _mediator = mediator;
        _portfolioRepository = portfolioRepository;
    }

    public async Task<Result<CreateRecommendedPortfolioResponse>> Handle(CreateRecommendedPortfolioCommand request, CancellationToken ct)
    {
        bool rebalancingActivated = false;
        var portfolio = RecommendedPortfolio.Create(request.Name, request.PortfolioItems.ToDictionary(x => x.Ticker), request.TerminationDate);
        if (portfolio.IsFailed)
            return Result.Fail<CreateRecommendedPortfolioResponse>(portfolio.Errors);

        var activeResult = await _portfolioRepository.GetActivePortfolioAsync(ct);
        PortfolioComparisonDTO? comparison = null;

        if (activeResult != null)
            comparison = ComparePortfolios(activeResult.Items, portfolio.Value.Items);

        await _portfolioRepository.AddAsync(portfolio.Value, ct);

        var saveResult = await _portfolioRepository.SaveChangesAsync(portfolio.Value.Id, ct);
        if (saveResult.IsFailed)
            return Result.Fail<CreateRecommendedPortfolioResponse>(saveResult.Errors);

        // DISPARA PROCESSO DE REBALANCEAMENTO
        if (comparison != null && comparison.HasChanges())
        {
            rebalancingActivated = true;
            var rebalanceResult = await _mediator.Send(new RebalancePortfolioCommand(comparison), ct);
            if (rebalanceResult.IsFailed)
                return Result.Fail<CreateRecommendedPortfolioResponse>(rebalanceResult.Errors);
        }

        return Result.Ok(CreateRecommendedPortfolioResponse.SuccessfullyCreated(portfolio.Value, rebalancingActivated));
    }

    private PortfolioComparisonDTO ComparePortfolios(
        IReadOnlyCollection<PortfolioItem> current,
        IReadOnlyCollection<PortfolioItem> incoming)
    {
        var currentAssetsDictionary = current.ToDictionary(x => x.Ticker);
        var incomingAssetsDictionary = incoming.ToDictionary(x => x.Ticker);

        var removedTickers = currentAssetsDictionary.Keys.Except(incomingAssetsDictionary.Keys);
        var addedTickers = incomingAssetsDictionary.Keys.Except(currentAssetsDictionary.Keys);

        var removed = removedTickers.Select(t => new RemovedItem(t)).ToList();
        var added = addedTickers.Select(t => new NewItem(t, incomingAssetsDictionary[t].Percentage)).ToList();

        var altered = currentAssetsDictionary.Keys.Intersect(incomingAssetsDictionary.Keys)
            .Where(t => currentAssetsDictionary[t].Percentage != incomingAssetsDictionary[t].Percentage)
            .Select(t => new AlteredItem(t, currentAssetsDictionary[t].Percentage, incomingAssetsDictionary[t].Percentage))
            .ToList();

        return new PortfolioComparisonDTO(altered, removed, added);

    }
}
