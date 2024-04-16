namespace Flyr.Supermarket.Domain.models.PricingRules;

public class PricingRule(Dictionary<string, PromoCondition> itemConditions, IDiscount discount)
    : IPricingRule
{
    private Dictionary<string, PromoCondition> ItemConditions { get; } = itemConditions;
    private IDiscount Discount { get; } = discount;
    
    public bool IsSatisfied(Dictionary<string, int> cart)
    {
        return ItemConditions.All(kv => cart.ContainsKey(kv.Key) && cart[kv.Key] >= kv.Value.MinimumRequired);
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

public record PromoCondition(int MinimumRequired, int? AffectedMax);