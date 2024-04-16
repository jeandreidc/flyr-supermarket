using Flyr.Supermarket.Domain.models;
using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Domain;

public class Checkout(List<IPricingRule> pricingRules) : ICheckout
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
        if (_cart.ContainsKey(product.ProductCode))
        {
            _cart[product.ProductCode] += 1;
        }
        else 
        {
            _cart[product.ProductCode] = 1;
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
            var originalPrice = discountResult!.DiscountedItems.Sum(kv => _catalog[kv.Key] * kv.Value);
            totalDiscount += discountResult.Discount.Calculate(originalPrice);
            
            foreach (var discountedItem in discountResult.DiscountedItems)
            {
                //_cart[discountedItem.Key] -= discountedItem.Value;

                if (_discountedCart.ContainsKey(discountedItem.Key)) {
                    
                    _discountedCart[discountedItem.Key] += discountedItem.Value;
                } else {
                    _discountedCart[discountedItem.Key] = discountedItem.Value;
                }
            }
        }

        return totalDiscount;
    }
}

public interface ICheckout
{
    void Scan(Product product);
    double Total();
}