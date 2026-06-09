namespace PortfolioScheduler.Application.Commands.UpdateMontlhyAmount;

public record UpdateMonthlyAmountResponse(
    long CustomerId,
    decimal LastMonthlyAmount,
    decimal NewMonthlyAmount)
{
    public DateTime UpdatedAt { get; init; } = DateTime.Now;
    public string Message { get; init; } = "Monthly amount updated successfully. The new value will be considered at next billing cycle.";
}
