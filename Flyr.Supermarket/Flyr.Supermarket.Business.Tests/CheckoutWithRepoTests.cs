using FluentAssertions;
using Flyr.Supermarket.Domain;
using Flyr.Supermarket.Domain.exceptions;
using Flyr.Supermarket.Domain.interfaces;
using Flyr.Supermarket.Domain.models;
using Flyr.Supermarket.Domain.models.DiscountApplier;
using Flyr.Supermarket.Domain.models.PricingRules;
using Moq;

namespace Flyr.Supermarket.Business.Tests;

public class CheckoutWithRepoTests
{
    private readonly Mock<ICatalogRepository> _mockedCatalogRepoMock;
    
    public CheckoutWithRepoTests()
    {
        _mockedCatalogRepoMock = new Mock<ICatalogRepository>();
        
        _mockedCatalogRepoMock.Setup(m => m.GetPriceDictionary())
            .Returns( new Dictionary<string, double>{
                    ["GR1"] = 3.11,
                    ["SR1"] = 5.00,
                    ["CF1"] = 11.23
                });

        var mockedProducts = new List<Product>
        {
            new("GR1", "Green tea", 3.11, "GBP"),
            new("SR1", "Strawberries", 5.00, "GBP"),
            new("CF1", "Coffee", 11.23, "GBP")
        };
        
        _mockedCatalogRepoMock.Setup(m => m.GetAllAsync())
            .ReturnsAsync(mockedProducts);

        _mockedCatalogRepoMock.Setup(m => m.GetByIdAsync(It.IsIn(mockedProducts.Select(mp => mp.ProductCode))))
            .ReturnsAsync((string s) => mockedProducts.Single(p => p.ProductCode == s));
        
        
        _mockedCatalogRepoMock.Setup(m => m.GetByIdAsync(It.IsNotIn(mockedProducts.Select(mp => mp.ProductCode))))
            .ReturnsAsync(null as Product);
        
    }
    
    [Fact]
    public void Total_GivenBuy1Take1GreenTeaPromo_Returns50PercentOff()
    { 
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition> {["GR1"] = new(2, 2)}, new NonBundleDiscountRate(0.5, DiscountType.Percentage))
        };

        var checkout = new CheckoutWithRepo(_mockedCatalogRepoMock.Object, new FirstMatchDiscountApplier(), pricingRules);
        checkout.Scan("GR1");
        checkout.Scan("GR1");

        var total = checkout.Total();
        total.Should().Be(3.11);
    }
    
    [Fact]
    public void Total_GivenDiscountedItemsGreaterThan3Coffee_ReturnsTwoThirdsDiscountedPrice()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition> {["GR1"] = new(2, 2)}, new NonBundleDiscountRate(0.5, DiscountType.Percentage)),
            new PricingRule(new Dictionary<string, PromoCondition> {["CF1"] = new(3, null)}, new NonBundleDiscountRate(1.0/3.0, DiscountType.Percentage))
        };
        
        var checkout = new CheckoutWithRepo(_mockedCatalogRepoMock.Object, new FirstMatchDiscountApplier(), pricingRules);
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
            new PricingRule(new Dictionary<string, PromoCondition> {["SR1"] = new(3, null)}, new NonBundleDiscountRate(.50/5.00, DiscountType.Percentage))
        };
        
        var checkout = new CheckoutWithRepo(_mockedCatalogRepoMock.Object, new FirstMatchDiscountApplier(), pricingRules);
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
            new PricingRule(new Dictionary<string, PromoCondition> {["SR1"] = new(3, null)}, new NonBundleDiscountRate(.50, DiscountType.WholeNumberOff))
        };
        
        var checkout = new CheckoutWithRepo(_mockedCatalogRepoMock.Object, new FirstMatchDiscountApplier(), pricingRules);
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
            new PricingRule(new Dictionary<string, PromoCondition> {["SR1"] = new(3, null)}, new NonBundleDiscountRate(.50/5.00, DiscountType.Percentage))
        };
        
        var checkout = new CheckoutWithRepo(_mockedCatalogRepoMock.Object, new FirstMatchDiscountApplier(), pricingRules);
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
            new PricingRule(new Dictionary<string, PromoCondition> {["GR1"] = new(2, 2)}, new NonBundleDiscountRate(0.5, DiscountType.Percentage)),
            new PricingRule(new Dictionary<string, PromoCondition> {["CF1"] = new(3, null)}, new NonBundleDiscountRate(1.0/3.0, DiscountType.Percentage)),
            new PricingRule(new Dictionary<string, PromoCondition> {["SR1"] = new(3, null)}, new NonBundleDiscountRate(.50/5.00, DiscountType.Percentage))
        };
        
        var checkout = new CheckoutWithRepo(_mockedCatalogRepoMock.Object, new FirstMatchDiscountApplier(), pricingRules);
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
            new PricingRule(new Dictionary<string, PromoCondition> {["GR1"] = new(2, 2)}, new NonBundleDiscountRate(0.5, DiscountType.Percentage)),
            new PricingRule(new Dictionary<string, PromoCondition> {["CF1"] = new(3, null)}, new NonBundleDiscountRate(1.0/3.0, DiscountType.Percentage)),
            new PricingRule(new Dictionary<string, PromoCondition> {["SR1"] = new(3, null)}, new NonBundleDiscountRate(.50/5.00, DiscountType.Percentage))
        };
        
        var checkout = new CheckoutWithRepo(_mockedCatalogRepoMock.Object, new FirstMatchDiscountApplier(), pricingRules);
        checkout.Invoking(y => y.Scan("INV000"))
            .Should().Throw<InvalidProductException>()
            .WithMessage($"Item code: INV000 does not exist in the catalog");
    }
    
    // Bundled Discount testing
    
    [Fact]
    public void Total_GivenBundledDiscountedItemsWithFixedPrice_ReturnsCorrectDiscountedPrice()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition>
            {
                ["GR1"] = new(1, 1),
                ["SR1"] = new(1, 1),
                ["CF1"] = new(1, 1),
            }, new BundledDiscountRate(5.0, DiscountType.LowerPrice)),};
        
        var checkout = new CheckoutWithRepo(_mockedCatalogRepoMock.Object, new FirstMatchDiscountApplier(), pricingRules);
        checkout.Scan("SR1");
        checkout.Scan("CF1");
        checkout.Scan("GR1");

        var total = checkout.Total();
        total.Should().BeApproximately(5.0, 0.01F);
    }
    
    [Fact]
    public void Total_GivenBundledDiscountedItemsWithPercentage_ReturnsCorrectDiscountedPrice()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition>
            {
                ["GR1"] = new(1, 1),
                ["SR1"] = new(1, 1),
                ["CF1"] = new(1, 1),
            }, new BundledDiscountRate(0.5, DiscountType.Percentage)),};
        
        var checkout = new CheckoutWithRepo(_mockedCatalogRepoMock.Object, new FirstMatchDiscountApplier(), pricingRules);
        checkout.Scan("SR1");
        checkout.Scan("CF1");
        checkout.Scan("GR1");

        var total = checkout.Total();
        
        /*
            ["GR1"] = 3.11,
            ["SR1"] = 5.00,
            ["CF1"] = 11.23
        */
        total.Should().BeApproximately((3.11 + 5.00 + 11.23) / 2, 0.01F);
    }
    
    
    [Fact]
    public void Total_GivenBundledDiscountedItemsWithWholeNumberOff_ReturnsCorrectDiscountedPrice()
    {
        var pricingRules = new List<IPricingRule>
        {
            new PricingRule(new Dictionary<string, PromoCondition>
            {
                ["GR1"] = new(1, 1),
                ["SR1"] = new(1, 1),
                ["CF1"] = new(1, 1),
            }, new BundledDiscountRate(10.0, DiscountType.WholeNumberOff)),};
        
        var checkout = new CheckoutWithRepo(_mockedCatalogRepoMock.Object, new FirstMatchDiscountApplier(), pricingRules);
        checkout.Scan("SR1");
        checkout.Scan("CF1");
        checkout.Scan("GR1");

        var total = checkout.Total();
        
        /*
            ["GR1"] = 3.11,
            ["SR1"] = 5.00,
            ["CF1"] = 11.23
        */
        total.Should().BeApproximately(3.11 + 5.00 + 11.23 - 10.0, 0.01F);
    }
}