using FluentAssertions;
using Flyr.Supermarket.Domain.models;
using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Business.Tests;

public class PricingRuleTests
{
    [Fact]
    public void HasSatisfiedCondition_WhenNoRuleWasSatisfied_ReturnsFalse()
    {
        var pricingRule = new PricingRule(new Dictionary<string, int>
        {
            ["CB1"] = 1
        }, new Discount(1.0, DiscountType.WholeNumber));

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
        var pricingRule = new PricingRule(new Dictionary<string, int>
        {
            ["CB1"] = 1
        }, new Discount(1.0, DiscountType.WholeNumber));

        var cart = new Dictionary<string, int>
        {
            ["CB1"] = 2
        };

        var result = pricingRule.IsSatisfied(cart);
        result.Should().BeTrue();
    }

    [Fact]
    public void GetDiscount_WhenRuleIsSatisfied_ReturnsDiscountResultAndDiscountedItems()
    {
        var discount = new Discount(1.0, DiscountType.WholeNumber);
        var pricingRule = new PricingRule(new Dictionary<string, int>
        {
            ["CB1"] = 1
        }, discount);

        var cart = new Dictionary<string, int>
        {
            ["CB1"] = 2
        };

        var applicableDiscount = pricingRule.GetApplicableDiscount(cart);
        applicableDiscount.Should().NotBeNull();
        applicableDiscount!.DiscountedItems.Should().NotBeEmpty();
        applicableDiscount!.Discount.Should().BeEquivalentTo(discount);
    }
    
}