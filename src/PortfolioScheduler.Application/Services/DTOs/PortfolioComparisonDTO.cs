namespace PortfolioScheduler.Application.Services.DTOs;

public record PortfolioComparisonDTO(
    List<AlteredItem> Altered,
    List<RemovedItem> Removed,
    List<NewItem> Added)
{
    public bool HasChanges() => Altered.Any() || Removed.Any() || Added.Any();
}

public record AlteredItem(
    string Ticker,
    decimal OldPercentage,
    decimal NewPercentage)
{
    public bool IsIncrease => NewPercentage > OldPercentage;
    public decimal PercentageDiff => NewPercentage - OldPercentage;
}

public record RemovedItem(
    string Ticker);

public record NewItem(
    string Ticker,
    decimal Percentage);