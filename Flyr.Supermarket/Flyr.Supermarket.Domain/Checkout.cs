using Flyr.Supermarket.Domain.exceptions;
using Flyr.Supermarket.Domain.models;
using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Domain;

public class Checkout(IEnumerable<IPricingRule> pricingRules) : ICheckout
{
    // TODO: We can use a cart class so we could apply discounts to the cart
    // Sa halip na sa loob ng pricing rule cincompute pwedeng icompute sa loob ng 
    // cart object
    private readonly Dictionary<string, int> _cart = [];

    // TODO: We can add catalog service here
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
        foreach (var pr in pricingRules)
        {
            var discountResult = pr.ApplyDiscounts(aggregatedCart, _catalog);
            aggregatedCart = discountResult.UpdatedCart;
            discounts.Add(discountResult);
        }

        return aggregatedCart.Sum(pair => _catalog[pair.Key] * aggregatedCart[pair.Key]) +
               discounts.Sum(d => d.TotalDiscountedPrice);
    }
}

public interface ICheckout
{
    void Scan(Product product);
    double Total();
}