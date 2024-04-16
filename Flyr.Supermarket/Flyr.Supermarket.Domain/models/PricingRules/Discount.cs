using Flyr.Supermarket.Domain.exceptions;

namespace Flyr.Supermarket.Domain.models.PricingRules;

public class Discount(double discountValue, DiscountType discountType) : IDiscount
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

public interface IDiscount
{
    double Calculate(double originalPrice);
}

public enum DiscountType
{
    WholeNumber,
    Percentage
}
public record DiscountResult(Dictionary<string, int> DiscountedItems, IDiscount Discount);