using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Domain.models.DiscountApplier;

public class FirstMatchDiscountApplier : IDiscountApplier
{
    public CartDiscountResult ApplyDiscounts(Cart cart, IEnumerable<IDiscount> applicableDiscounts, Dictionary<string, double> priceDictionary)
    {
        var cartClone = cart.Clone();
        return applicableDiscounts.Aggregate(
            new CartDiscountResult(cartClone, new Cart(), 0.0), (current, applicableDiscount) 
                => applicableDiscount.Apply(current, priceDictionary));
    }
}