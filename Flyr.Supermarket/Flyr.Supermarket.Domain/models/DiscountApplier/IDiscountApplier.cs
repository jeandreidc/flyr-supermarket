using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Domain.models.DiscountApplier;

public interface IDiscountApplier
{
    CartDiscountResult ApplyDiscounts(Cart cart, IEnumerable<IDiscount> applicableDiscounts, Dictionary<string, double> getPriceDictionary);
}