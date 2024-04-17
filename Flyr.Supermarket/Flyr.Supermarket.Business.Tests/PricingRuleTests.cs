using FluentAssertions;
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
        }, new NonBundleDiscountRate(1.0, DiscountType.WholeNumberOff));

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
        }, new NonBundleDiscountRate(1.0, DiscountType.WholeNumberOff));

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
        var discount = new NonBundleDiscountRate(1.0, DiscountType.WholeNumberOff);
        var pricingRule = new PricingRule(new Dictionary<string, PromoCondition>
        {
            ["CB1"] = new(1, 1)
        }, discount);

        var cart = new Dictionary<string, int>
        {
            ["CB1"] = 2
        };

        var applicableDiscount = pricingRule.ApplyDiscount(cart, new Dictionary<string, double>{["CB1"] = 1.0});
        applicableDiscount.Should().NotBeNull();
        applicableDiscount!.DiscountedItems.Should().NotBeEmpty();
        applicableDiscount!.Savings.Should().Be(2.0);
        applicableDiscount!.UpdatedCart.Should().ContainKey("CB1").WhoseValue.Should().Be(0);
    }
    
}