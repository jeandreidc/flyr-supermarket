using Flyr.Supermarket.Domain.models;
using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Domain;

public interface IDiscountApplier
{
    CartDiscountResult ApplyDiscounts(Cart cart, IEnumerable<IDiscount> applicableDiscounts, Dictionary<string, double> getPriceDictionary);
}

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

public class SmartDiscountApplier : IDiscountApplier
{
    // TODO Create a DP solution to return the best discount
    public CartDiscountResult ApplyDiscounts(Cart cart, IEnumerable<IDiscount> applicableDiscounts, Dictionary<string, double> getPriceDictionary)
    {
        throw new NotImplementedException();
    }
}