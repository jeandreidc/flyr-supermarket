using FluentAssertions;
using Flyr.Supermarket.Domain;
using Flyr.Supermarket.Domain.exceptions;
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
            new PricingRule(new Dictionary<string, PromoCondition> {["GR1"] = new(2, 2)}, new DiscountCalculatorCalculator(0.5, DiscountType.Percentage))
        };
        
        var checkout = new Checkout(pricingRules);
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));

        var total = checkout.Total();
        total.Should().BeApproximately(6.22, 0.01F);
    }
    
    [Fact]
    public void Total_GivenDiscountedItemsBuyOneTakeOneTwice_ReturnsDiscountedPrice()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition> {["GR1"] = new(2, 2)}, new DiscountCalculatorCalculator(0.5, DiscountType.Percentage))
        };
        
        var checkout = new Checkout(pricingRules);
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));
        checkout.Scan(new Product("GR1", "Green tea", 3.11, "GBP"));

        var total = checkout.Total();
        total.Should().BeApproximately(6.22, 0.01F);
    }
    
    [Fact]
    public void Total_GivenDiscountedItemsGreaterThan3Coffee_ReturnsTwoThirdsDiscountedPrice()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition> {["GR1"] = new(2, 2)}, new DiscountCalculatorCalculator(0.5, DiscountType.Percentage)),
            new PricingRule(new Dictionary<string, PromoCondition> {["CF1"] = new(3, null)}, new DiscountCalculatorCalculator(1.0/3.0, DiscountType.Percentage))
        };
        
        var checkout = new Checkout(pricingRules);
        checkout.Scan("CF1");
        checkout.Scan("CF1");
        checkout.Scan("CF1");
        checkout.Scan("CF1");

        var total = checkout.Total();
        total.Should().BeApproximately(29.95, 0.01F);
    }
    
    [Fact]
    public void Total_GivenDiscountedItemsGreaterThan3Strawberries_Returns450DiscountedPrice()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition> {["SR1"] = new(3, null)}, new DiscountCalculatorCalculator(.50/5.00, DiscountType.Percentage))
        };
        
        var checkout = new Checkout(pricingRules);
        checkout.Scan("SR1");
        checkout.Scan("SR1");
        checkout.Scan("SR1");
        checkout.Scan("SR1");

        var total = checkout.Total();
        total.Should().BeApproximately(18.00, 0.01F);
    }
    
    
    [Fact]
    public void Total_GivenDiscountedItemsGreaterThan3Strawberries_ThenUsingWholeNumberDiscount_ReturnsSameDiscountedPrice()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition> {["SR1"] = new(3, null)}, new DiscountCalculatorCalculator(.50, DiscountType.WholeNumber))
        };
        
        var checkout = new Checkout(pricingRules);
        checkout.Scan("SR1");
        checkout.Scan("SR1");
        checkout.Scan("SR1");
        checkout.Scan("SR1");

        var total = checkout.Total();
        total.Should().BeApproximately(18.00, 0.01F);
    }
    
    [Fact]
    public void Total_GivenNonDiscountedIStrawberries_ReturnsNonDiscountedPrice()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition> {["SR1"] = new(3, null)}, new DiscountCalculatorCalculator(.50/5.00, DiscountType.Percentage))
        };
        
        var checkout = new Checkout(pricingRules);
        checkout.Scan("SR1");
        checkout.Scan("SR1");

        var total = checkout.Total();
        total.Should().BeApproximately(10.00, 0.01F);
    }
    
    [Fact]
    public void Total_GivenMultipleDiscountedItems_ReturnsCorrectDiscountedPrice()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition> {["GR1"] = new(2, 2)}, new DiscountCalculatorCalculator(0.5, DiscountType.Percentage)),
            new PricingRule(new Dictionary<string, PromoCondition> {["CF1"] = new(3, null)}, new DiscountCalculatorCalculator(1.0/3.0, DiscountType.Percentage)),
            new PricingRule(new Dictionary<string, PromoCondition> {["SR1"] = new(3, null)}, new DiscountCalculatorCalculator(.50/5.00, DiscountType.Percentage))
        };
        
        var checkout = new Checkout(pricingRules);
        checkout.Scan("SR1");
        checkout.Scan("SR1");
        checkout.Scan("SR1");
        checkout.Scan("SR1");
        checkout.Scan("CF1");
        checkout.Scan("CF1");
        checkout.Scan("CF1");
        checkout.Scan("CF1");
        checkout.Scan("GR1");
        checkout.Scan("GR1");
        checkout.Scan("GR1");

        var total = checkout.Total();
        total.Should().BeApproximately(18.00 + 29.95 + 6.22, 0.01F);
    }

    [Fact]
    public void Scan_GivenInvalidProduct_ThrowsException()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition> {["GR1"] = new(2, 2)}, new DiscountCalculatorCalculator(0.5, DiscountType.Percentage)),
            new PricingRule(new Dictionary<string, PromoCondition> {["CF1"] = new(3, null)}, new DiscountCalculatorCalculator(1.0/3.0, DiscountType.Percentage)),
            new PricingRule(new Dictionary<string, PromoCondition> {["SR1"] = new(3, null)}, new DiscountCalculatorCalculator(.50/5.00, DiscountType.Percentage))
        };
        
        var checkout = new Checkout(pricingRules);
        checkout.Invoking(y => y.Scan("INV000"))
            .Should().Throw<InvalidProductException>()
            .WithMessage($"Item code: INV000 does not exist in the catalog");
    }
}