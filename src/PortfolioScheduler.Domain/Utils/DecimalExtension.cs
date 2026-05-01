namespace PortfolioScheduler.Domain.Utils;

public static class DecimalExtension
{
    public static decimal ToMoney(this decimal value)
    {
        return Math.Round(value, 2);
    }

    public static decimal ToPercentage(this decimal value)
    {
        return Math.Round(value, 2);
    }
}