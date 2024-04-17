using Flyr.Supermarket.Domain.exceptions;

namespace Flyr.Supermarket.Domain.models.PricingRules;

public class DiscountCalculatorCalculator(double discountValue, DiscountType discountType) : IDiscountCalculator
{
    public double Calculate(double originalPrice)
    {
        return discountType switch
        {
            DiscountType.WholeNumber => discountValue,
            DiscountType.Percentage => originalPrice * discountValue,
            _ => throw new InvalidDiscountException(discountType)
        };
    }
}

public interface IDiscountCalculator
{
    double Calculate(double originalPrice);
}

public enum DiscountType
{
    WholeNumber,
    Percentage
}
public record DiscountResult(Dictionary<string, int> UpdatedCart, Dictionary<string, int> DiscountedItems, double TotalDiscountedPrice);