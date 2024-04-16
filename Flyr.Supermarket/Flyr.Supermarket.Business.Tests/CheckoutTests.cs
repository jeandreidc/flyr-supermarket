using FluentAssertions;
using Flyr.Supermarket.Domain;
using Flyr.Supermarket.Domain.models;
using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Business.Tests;

/*
 Test Cases
 1. No discount simple addition
 2. Green tea discount x1
 3. 2 discount different type
 4. 2 of the same discount
 */
public class CheckoutTests
{
    [Fact]
    public void Total_NoDiscountedItems_ReturnsCorrectSum()
    {
        var pricingRules = new List<IPricingRule>();
        var checkout = new Checkout(pricingRules);
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));
        checkout.Scan(new Product("SR1", "Strawberries", 5.00, "GBP"));

        var total = checkout.Total();
        total.Should().Be(8.11);
    }
    
    [Fact]
    public void Total_GivenBuy1Take1GreenTeaPromo_Returns50PercentOff()
    {
        var pricingRules = new List<IPricingRule>();
        var checkout = new Checkout(pricingRules);
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));
        checkout.Scan(new Product("SR1", "Strawberries", 5.00, "GBP"));

        var total = checkout.Total();
        total.Should().Be(8.11);
    }
    
    [Fact]
    public void Total_GivenDiscountedItemsBuyOneTakeOne_ReturnsDiscountedPrice()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, int> {["GR1"] = 2}, new Discount(0.5, DiscountType.Percentage))
        };
        
        var checkout = new Checkout(pricingRules);
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));

        var total = checkout.Total();
        total.Should().BeApproximately(6.22, 0.01F);
    }
}