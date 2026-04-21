using FluentResults;
using PortfolioScheduler.Domain.Services.DTOs;

namespace PortfolioScheduler.Domain.Services.Interfaces;

public interface ICotahistParser
{
    IEnumerable<QuoteDTO> ParseArquivo(HashSet<string> tickers, string caminhoArquivo);
    Result<string> ExistsCotahist(DateOnly referenceDate);
}
