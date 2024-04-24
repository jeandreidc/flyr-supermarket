using Flyr.Supermarket.Domain.exceptions;
using Flyr.Supermarket.Domain.interfaces;
using Flyr.Supermarket.Domain.models;
using Flyr.Supermarket.Domain.models.DiscountApplier;
using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Domain;

public class CheckoutWithRepo(ICatalogRepository catalogRepository, IDiscountApplier discountApplier, IEnumerable<IPricingRule> pricingRules)
    : ICheckout
{
    private readonly Cart _cart = new();

    public void Scan(Product product)
    {
        throw new NotImplementedException();
    }

    public void Scan(string productCode)
    {
        var product = catalogRepository.GetByIdAsync(productCode).GetAwaiter().GetResult();
        if (product == null) throw new InvalidProductException(productCode);
        
        _cart.Add(product);
    }

    public double Total()
    {
        var priceDictionary = catalogRepository.GetPriceDictionary();
        var applicableDiscounts = pricingRules.SelectMany(pr => pr.GetApplicableDiscounts(_cart)).ToList();
        var discountResult = discountApplier.ApplyDiscounts(_cart, applicableDiscounts, priceDictionary);
        return discountResult.UpdatedCart.ComputeTotalPrice(priceDictionary) + discountResult.DiscountedItems.ComputeTotalPrice(priceDictionary) - discountResult.Savings;
    }
}