using Flyr.Supermarket.Domain.exceptions;
using Flyr.Supermarket.Domain.models;
using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Domain;

public class Checkout(IEnumerable<IPricingRule> pricingRules) : ICheckout
{
    private readonly Dictionary<string, int> _cart = [];
    private readonly Dictionary<string, int> _discountedCart = [];

    private readonly Dictionary<string, double> _catalog = new()
    {
        ["GR1"] = 3.11,
        ["SR1"] = 5.00,
        ["CF1"] = 11.23
    };

    public void Scan(Product product)
    {
        if (!_cart.TryAdd(product.ProductCode, 1))
        {
            _cart[product.ProductCode] += 1;
        }
    }
    
    public void Scan(string itemCode)
    {
        if (!_catalog.ContainsKey(itemCode)) throw new InvalidProductException(itemCode);
        if (!_cart.TryAdd(itemCode, 1))
        {
            _cart[itemCode] += 1;
        }
    }

    public double Total()
    {
        var totalDiscount = CalculateTotalDiscount();
        return _cart.Sum(kv => _catalog[kv.Key] * kv.Value) - totalDiscount;
    }

    private double CalculateTotalDiscount()
    {
        var discounts = pricingRules
            .Select(pr => pr.GetApplicableDiscount(_cart))
            .Where(d => d != null);

        var totalDiscount = 0.0;
        
        foreach (var discountResult in discounts)
        {
            totalDiscount += discountResult!.DiscountedItems.Sum(kv => 
                discountResult.Discount.Calculate(_catalog[kv.Key]) * (kv.Value.AffectedMax ?? _cart[kv.Key]));
        }
        return totalDiscount;
    }
}

public interface ICheckout
{
    void Scan(Product product);
    double Total();
}