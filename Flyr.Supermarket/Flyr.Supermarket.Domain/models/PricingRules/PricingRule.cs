namespace Flyr.Supermarket.Domain.models.PricingRules;

public class PricingRule(Dictionary<string, int> itemConditions, IDiscount discount)
    : IPricingRule
{
    private Dictionary<string, int> ItemConditions { get; } = itemConditions;
    private IDiscount Discount { get; } = discount;
    
    public bool IsSatisfied(Dictionary<string, int> cart)
    {
        return ItemConditions.All(kv => cart.ContainsKey(kv.Key) && cart[kv.Key] >= kv.Value);
    }

    public DiscountResult? GetApplicableDiscount(Dictionary<string, int> cart)
    {
        return IsSatisfied(cart) ? new DiscountResult(ItemConditions, Discount) : null;
    }
}

public interface IPricingRule
{
    bool IsSatisfied(Dictionary<string, int> cart);
    DiscountResult? GetApplicableDiscount(Dictionary<string, int> cart);
}