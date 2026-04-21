using FluentResults;
using Microsoft.Extensions.Configuration;
using PortfolioScheduler.Domain.Services.DTOs;
using PortfolioScheduler.Domain.Services.Interfaces;

namespace PortfolioScheduler.Infra.ExternalData.B3;

public class CotahistParser : ICotahistParser
{
    private readonly IConfiguration _configuration;
    public CotahistParser(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<QuoteDTO> ParseArquivo(HashSet<string> tickers, string caminhoArquivo)
    {
        var cotacoes = new List<QuoteDTO>();

        foreach (var linha in File.ReadLines(caminhoArquivo))
        {
            // Ignorar header (00) e trailer (99)
            if (linha.Length < 245)
                continue;

            var tipoRegistro = linha.Substring(0, 2);
            if (tipoRegistro != "01")
                continue;

            var tipoMercado = int.Parse(linha.Substring(24, 3).Trim());

            // Filtrar apenas mercado a vista (010) e fracionario (020)
            if (tipoMercado != 10 && tipoMercado != 20)
                continue;

            var BDI = linha.Substring(10, 2).Trim();

            var ticker = linha.Substring(12, 12).Trim();

            if (BDI.Equals("96"))
            {
                // Ajustar ticker fracionário para o formato do portfólio recomendado
                ticker = ticker.Substring(0, ticker.Length - 1);
                if (!tickers.Contains(ticker))
                    continue;
            }

            // Filtrar apenas os tickers do portfólio recomendado
            if (!tickers.Contains(ticker))
                continue;

            var cotacao = new QuoteDTO
            {
                DataPregao = DateTime.ParseExact(
                    linha.Substring(2, 8), "yyyyMMdd",
                    System.Globalization.CultureInfo.InvariantCulture),
                BDICode = linha.Substring(10, 2).Trim(),
                Ticker = linha.Substring(12, 12).Trim(),
                MarketType = tipoMercado,
                CompanyName = linha.Substring(27, 12).Trim(),
                OpenPrice = ParsePreco(linha.Substring(56, 13)),
                MaxPrice = ParsePreco(linha.Substring(69, 13)),
                MinPrice = ParsePreco(linha.Substring(82, 13)),
                AveragePrice = ParsePreco(linha.Substring(95, 13)),
                ClosePrice = ParsePreco(linha.Substring(108, 13)),
                TradeQuantity = long.Parse(linha.Substring(152, 18).Trim()),
                TradedVolume = ParsePreco(linha.Substring(170, 18))
            };

            cotacoes.Add(cotacao);
        }

        return cotacoes;
    }

     private decimal ParsePreco(string valorBruto)
    {
        if (long.TryParse(valorBruto.Trim(), out var valor))
            return valor / 100m;
        return 0m;
    }

    public Result<string> ExistsCotahist(DateOnly referenceDate)
    {
        var caminhoArquivo = string.Concat(_configuration["B3Settings:CotahistFilePath"], referenceDate.ToString("ddMMyyyy"), ".TXT");
        if (!File.Exists(caminhoArquivo))
        {
            return Result.Fail(new Error("Arquivo de cotações não encontrado.")
                .WithMetadata("statusCode", 404));
        }
        return Result.Ok(caminhoArquivo);
    }
}
