using Flyr.Supermarket.Domain.exceptions;
using Flyr.Supermarket.Domain.interfaces;
using Flyr.Supermarket.Domain.models;
using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Domain;

public class CheckoutBasic(IEnumerable<IPricingRule> PricingRules) : ICheckout
{
    // Sa halip na sa loob ng pricing rule cincompute pwedeng icompute sa loob ng 
    // cart object
    private readonly Dictionary<string, int> _cart = [];

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
        var discounts = new List<DiscountResult>();
        var aggregatedCart = new Dictionary<string, int>(_cart);
        
        // We can extract this to a discount calculator service
        foreach (var pr in PricingRules)
        {
            var discountResult = pr.ApplyDiscount(aggregatedCart, _catalog);
            aggregatedCart = discountResult.UpdatedCart;
            discounts.Add(discountResult);
        }

        return _cart.Sum(pair => _catalog[pair.Key] * pair.Value) -
               discounts.Sum(d => d.Savings);
    }
}
