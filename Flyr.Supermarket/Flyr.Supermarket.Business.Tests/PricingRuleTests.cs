using FluentAssertions;
using Flyr.Supermarket.Domain.models;
using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Business.Tests;

public class PricingRuleTests
{
    [Fact]
    public void HasSatisfiedCondition_WhenNoRuleWasSatisfied_ReturnsFalse()
    {
        var pricingRule = new PricingRule(new Dictionary<string, PromoCondition>
        {
            ["CB1"] = new(1,1)
        }, new DiscountCalculatorCalculator(1.0, DiscountType.WholeNumber));

        var cart = new Dictionary<string, int>
        {
            ["CB2"] = 2
        };

        var result = pricingRule.IsSatisfied(cart);
        result.Should().BeFalse();
    }
    
    [Fact]
    public void HasSatisfiedCondition_WhenRuleWasSatisfied_ReturnsTrue()
    {
        var pricingRule = new PricingRule(new Dictionary<string, PromoCondition>
        {
            ["CB1"] = new(1, 1)
        }, new DiscountCalculatorCalculator(1.0, DiscountType.WholeNumber));

        var cart = new Dictionary<string, int>
        {
            ["CB1"] = 2
        };

        var result = pricingRule.IsSatisfied(cart);
        result.Should().BeTrue();
    }

    [Fact]
    public void GetDiscount_WhenExactNumberRuleIsSatisfied_ReturnsDiscountResultAndDiscountedItems()
    {
        var discount = new DiscountCalculatorCalculator(1.0, DiscountType.WholeNumber);
        var pricingRule = new PricingRule(new Dictionary<string, PromoCondition>
        {
            ["CB1"] = new(1, 1)
        }, discount);

        var cart = new Dictionary<string, int>
        {
            ["CB1"] = 2
        };

        var applicableDiscount = pricingRule.ApplyDiscounts(cart, new Dictionary<string, double>{["CB1"] = 1.0});
        applicableDiscount.Should().NotBeNull();
        applicableDiscount!.DiscountedItems.Should().NotBeEmpty();
        applicableDiscount!.TotalDiscountedPrice.Should().Be(0.0);
        applicableDiscount!.UpdatedCart.Should().ContainKey("CB1").WhoseValue.Should().Be(0);
    }
    
}