using Flyr.Supermarket.Domain.exceptions;

namespace Flyr.Supermarket.Domain.models.PricingRules;

public class NonBundleDiscountRate(double discountValue, DiscountType discountType) : IDiscountRate
{ 
    public double Calculate(Dictionary<string, int> discountedItems, Dictionary<string, double> catalogPriceList)
    {
        return discountType switch
        {
            DiscountType.WholeNumberOff => discountedItems.Sum(kv => discountValue * kv.Value),
            DiscountType.Percentage =>  discountedItems.Sum(kv => catalogPriceList[kv.Key] * discountValue * kv.Value),
            _ => throw new InvalidDiscountException(discountType)
        };
    }
}

public class BundledDiscountRate(double discountValue, DiscountType discountType) : IDiscountRate
{ 
    public double Calculate(Dictionary<string, int> discountedItems, Dictionary<string, double> catalogPriceList)
    {
        return discountType switch
        {
            DiscountType.LowerPrice => discountedItems.Sum(kv => catalogPriceList[kv.Key] * kv.Value) - discountValue,
            DiscountType.WholeNumberOff => discountValue,
            DiscountType.Percentage =>  discountedItems.Sum(kv => catalogPriceList[kv.Key] * kv.Value) * discountValue,
            _ => throw new InvalidDiscountException(discountType)
        };
    }
}

public interface IDiscountRate
{
    double Calculate(Dictionary<string, int> discountedItems, Dictionary<string, double> catalogRepository);
}

public enum DiscountType
{
    WholeNumberOff,
    Percentage,
    LowerPrice,
}