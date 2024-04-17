namespace Flyr.Supermarket.Domain.models.PricingRules;

public class Discount(Dictionary<string, int> itemConditions, IDiscountRate discountRate) : IDiscount
{ 
    public CartDiscountResult Apply(CartDiscountResult previousResult, Dictionary<string, double> catalogRepository)
    {
        if (!previousResult.UpdatedCart.Contains(itemConditions)) return previousResult;
        
        var updatedCart = previousResult.UpdatedCart.Remove(itemConditions);
        var updatedDiscountedItems = previousResult.DiscountedItems.Merge(itemConditions);
        return new CartDiscountResult(updatedCart, updatedDiscountedItems, previousResult.Savings + discountRate.Calculate(itemConditions, catalogRepository));

    }
}

public interface IDiscount
{
    CartDiscountResult Apply(CartDiscountResult previousResult, Dictionary<string, double> catalogRepository);
}

public record DiscountResult(Dictionary<string, int> UpdatedCart, Dictionary<string, int> DiscountedItems, double Savings);
public record CartDiscountResult(Cart UpdatedCart, Cart DiscountedItems, double Savings);