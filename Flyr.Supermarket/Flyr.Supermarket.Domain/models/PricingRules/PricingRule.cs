namespace Flyr.Supermarket.Domain.models.PricingRules;

public class PricingRule(Dictionary<string, PromoCondition> itemConditions, IDiscountRate discountRate)
    : IPricingRule
{
    // TODO Andrei: Add checking if SKU is existing
    private Dictionary<string, PromoCondition> ItemConditions { get; } = itemConditions;
    private IDiscountRate DiscountRate { get; } = discountRate;
    
    public bool IsSatisfied(Dictionary<string, int> cart)
    {
        return ItemConditions.All(kv => cart.ContainsKey(kv.Key) && cart[kv.Key] >= kv.Value.MinimumRequired);
    }

    public DiscountResult ApplyDiscount(Dictionary<string, int> cart, Dictionary<string, double> catalog)
    {
        var cartCopy = new Dictionary<string, int>(cart);
        var discountedGroup = new Dictionary<string, int>();
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
            }
        }
        return new DiscountResult(cartCopy, discountedGroup, DiscountRate.Calculate(discountedGroup, catalog));
    }

    public IEnumerable<IDiscount> GetApplicableDiscounts(Cart cart)
    {
        var cartCopy = cart.Clone();
        while (cartCopy.Contains(ItemConditions.ToDictionary(kv => kv.Key, kv => kv.Value.MinimumRequired)))
        {
            var discountedItems = ItemConditions.ToDictionary(i => i.Key, i => i.Value.AffectedMax ?? cart[i.Key]);
            cartCopy = cartCopy.Remove(discountedItems);
            yield return new Discount(discountedItems, DiscountRate);
        }
    }
}

public interface IPricingRule
{
    DiscountResult ApplyDiscount(Dictionary<string, int> cart, Dictionary<string, double> catalog);
    IEnumerable<IDiscount> GetApplicableDiscounts(Cart cart);
}

public record PromoCondition(int MinimumRequired, int? AffectedMax); // AffectedMax is null if it's a greedy discount