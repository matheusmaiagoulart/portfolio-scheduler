using Microsoft.EntityFrameworkCore;
using PortfolioScheduler.Application.Queries.Contracts;
using PortfolioScheduler.Application.Queries.GetCustomerPortfolio;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Utils;
using PortfolioScheduler.Infra.Data.Context;

namespace PortfolioScheduler.Infra.Queries;

public class CustomerReadRepository : ICustomerReadRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerReadRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetCustomerPortfolioResponse> GetCustomerPortfolio(long customerId, CancellationToken ct)
    {
        var customer = await _dbContext.Customers
        .AsNoTracking()
        .Include(c => c.BrokerageAccount)
            .ThenInclude(ba => ba.Custodies)
            .FirstOrDefaultAsync(c => c.Id == customerId, ct);

        if (customer == null) 
            return null;

        var tickers = customer.BrokerageAccount.Custodies.Select(c => c.Ticker).ToList();
        var currentPrices = await _dbContext.AssetPrices
            .AsNoTracking()
            .Where(a => tickers.Contains(a.Ticker))
            .GroupBy(a => a.Ticker)
            .Select(g => new
            {
                Ticker = g.Key,
                Price = g.OrderByDescending(a => a.TradingDate)
                 .Select(a => a.ClosePrice)
                 .First()
            })
            .ToDictionaryAsync(x => x.Ticker, x => x.Price, ct);

        var custodies = customer.BrokerageAccount.Custodies;

        var portfolioCurrentValue = Custody.CalcPortfolioTotalValue(custodies, currentPrices).ToMoney();
        var totalInvestedAmount = Custody.CalcTotalInvestedAmount(custodies).ToMoney();
        var plTotal = custodies.Select(custody => custody.CalcPL(currentPrices[custody.Ticker])).Sum().ToMoney();
        var plPercentage = Custody.CalcPortfolioProfitability(portfolioCurrentValue, totalInvestedAmount).ToMoney();

        return new GetCustomerPortfolioResponse
        (
            CustomerId: customerId,
            Name: customer.Name,
            BrokerageAccount: customer.BrokerageAccount.Id.ToString(),
            ConsultDate: DateTime.UtcNow,
            Resume: new Resume
            (
                TotalInvestedAmount: totalInvestedAmount,
                PortfolioCurrentValue: portfolioCurrentValue,
                PlTotal: plTotal,
                PlPercentage: plPercentage
            ),
            Assets: new Assets
            (
                AssetDetailsList: custodies.Select(custody =>
                {
                    var currentPrice = currentPrices[custody.Ticker].ToMoney();

                    return new AssetDetails(
                        Ticker: custody.Ticker,
                        Quantity: custody.Quantity,
                        AveragePrice: custody.AveragePrice,
                        CurrentPrice: currentPrice,
                        Pl: custody.CalcPL(currentPrice).ToMoney(),
                        PlPercentage: custody.CalcPlPercentual(currentPrice).ToPercentage(),
                        PortfolioCompositionPercentage: custody.CalcCompositionPercentage(currentPrice, portfolioCurrentValue).ToPercentage());
                }).ToList()
            )
        );
    }
}
