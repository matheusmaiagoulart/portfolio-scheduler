namespace PortfolioScheduler.Application.Queries.GetCustomerPortfolio;

public record GetCustomerPortfolioResponse(
    long CustomerId,
    string Name,
    string BrokerageAccount,
    DateTime ConsultDate,
    Resume Resume,
    Assets Assets);

public record Resume(
    decimal TotalInvestedAmount,
    decimal PortfolioCurrentValue,
    decimal PlTotal,
    decimal PlPercentage);

public record Assets(
    List<AssetDetails> AssetDetailsList);

public record AssetDetails(
    string Ticker,
    int Quantity,
    decimal AveragePrice,
    decimal CurrentPrice,
    decimal Pl,
    decimal PlPercentage,
    decimal PortfolioCompositionPercentage);
