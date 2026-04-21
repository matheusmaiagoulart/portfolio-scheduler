using FluentResults;
using MediatR;
using PortfolioScheduler.Domain.DomainErrors;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Domain.Services.DTOs;
using PortfolioScheduler.Domain.Services.Interfaces;

namespace PortfolioScheduler.Application.Commands.ImportStockQuotes;

public class ImportStockQuotesHandler : IRequestHandler<ImportStockQuotesCommand, Result<IEnumerable<QuoteDTO>>>
{
    private readonly ICotahistParser _cotahistParser;
    private readonly IAssetPricesRepository _assetPricesRepository;
    private readonly IPurchaseExecutionDateValidator _purchaseExecutionDateValidator;
    private readonly IRecommendedPortfolioRepository _recommendedPortfolioRepository;
    public ImportStockQuotesHandler(ICotahistParser cotahistParser, IAssetPricesRepository assetPricesRepository, IPurchaseExecutionDateValidator purchaseExecutionDateValidator, IRecommendedPortfolioRepository recommendedPortfolioRepository)
    {
        _cotahistParser = cotahistParser;
        _assetPricesRepository = assetPricesRepository;
        _purchaseExecutionDateValidator = purchaseExecutionDateValidator;
        _recommendedPortfolioRepository = recommendedPortfolioRepository;
    }

    public async Task<Result<IEnumerable<QuoteDTO>>> Handle(ImportStockQuotesCommand request, CancellationToken ct)
    {
        //var validateDateResult = _purchaseExecutionDateValidator.ValidatePurchaseDay(DateTime.Now);
        //if (validateDateResult.IsFailed)
        //    return Result.Fail(validateDateResult.Errors);

        var caminhoArquivo = _cotahistParser.ExistsCotahist(request.ReferenceDate);
        if (caminhoArquivo.IsFailed)
        {
            return Result.Fail(new Error("Arquivo de cotações não encontrado.")
                .WithMetadata("statusCode", 404));
        }

        var portofolio = await _recommendedPortfolioRepository.GetRecommendedPortfolioActive(ct);
        if (portofolio is null || !portofolio.Items.Any())
            return Result.Fail(PortfolioErrors.NoActiveRecommendedPortfolio());

        var tickers = portofolio.Items.Select(p => p.Ticker).ToHashSet();

        var resultCotahistParse = _cotahistParser.ParseArquivo(tickers, caminhoArquivo.Value);
        if (!resultCotahistParse.Any())
            return Result.Fail(new Error("Nenhuma cotação encontrada no arquivo.").WithMetadata("statusCode", 400));

        var assetPrices = resultCotahistParse.Select(x => AssetPrices.CreateAssetPrice(
               x.DataPregao,
               x.Ticker,
               x.OpenPrice,
               x.ClosePrice,
               x.MaxPrice,
               x.MinPrice
               ));

        await _assetPricesRepository.AddAsync(assetPrices);

        var resultSave = await _assetPricesRepository.SaveChangesAsync(assetPrices, ct);
        if (resultSave.IsFailed)
            return Result.Fail(resultSave.Errors);

        return Result.Ok(resultCotahistParse);
    }
}
