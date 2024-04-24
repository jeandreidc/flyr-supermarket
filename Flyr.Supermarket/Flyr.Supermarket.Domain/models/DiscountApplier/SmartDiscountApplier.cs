using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Domain.models.DiscountApplier;

public class SmartDiscountApplier : IDiscountApplier
{
    // TODO Create a DP solution to return the best discount
    public CartDiscountResult ApplyDiscounts(Cart cart, IEnumerable<IDiscount> applicableDiscounts, Dictionary<string, double> getPriceDictionary)
    {
        var cartClone = cart.Clone();
        
    }
}