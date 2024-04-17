namespace Flyr.Supermarket.Domain.models.PricingRules;

public class PricingRule(Dictionary<string, PromoCondition> itemConditions, IDiscountCalculator discountCalculator)
    : IPricingRule
{
    // TODO Andrei: Add checking if SKU is existing
    private Dictionary<string, PromoCondition> ItemConditions { get; } = itemConditions;
    private IDiscountCalculator DiscountCalculator { get; } = discountCalculator;
    
    public bool IsSatisfied(Dictionary<string, int> cart)
    {
        return ItemConditions.All(kv => cart.ContainsKey(kv.Key) && cart[kv.Key] >= kv.Value.MinimumRequired);
    }

    public DiscountResult ApplyDiscounts(Dictionary<string, int> cart, Dictionary<string, double> catalog)
    {
        var cartCopy = new Dictionary<string, int>(cart);
        var discountedGroup = new Dictionary<string, int>(cart);
        var discountedPrice = 0.0;
        while (IsSatisfied(cartCopy))
        {
            foreach (var product in ItemConditions.Keys)
            {
                var noOfProductsAffected = ItemConditions[product].AffectedMax ?? cartCopy[product];
                cartCopy[product] -= noOfProductsAffected;
                if(!discountedGroup.TryAdd(product, noOfProductsAffected))
                {
                    discountedGroup[product] += noOfProductsAffected;
                }

                discountedPrice += (catalog[product] - DiscountCalculator.Calculate(catalog[product])) * noOfProductsAffected;
            }
        }
        return new DiscountResult(cartCopy, discountedGroup, discountedPrice);
    }
}

public interface IPricingRule
{
    bool IsSatisfied(Dictionary<string, int> cart);
    DiscountResult ApplyDiscounts(Dictionary<string, int> cart, Dictionary<string, double> catalog);
}

public record PromoCondition(int MinimumRequired, int? AffectedMax); // AffectedMax is null if it's a greedy discount