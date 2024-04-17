using FluentAssertions;
using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Business.Tests;

public class DiscountCalculatorCalculatorTests
{
    [Fact]
    public void Calculate_WhenWholeNumber_ReturnsCorrectValue()
    {
        var discount = new DiscountCalculatorCalculator(0.5, DiscountType.WholeNumber);
        var totalDiscount = discount.Calculate(2.0);
        totalDiscount.Should().Be(0.50);
    }
    
    [Fact]
    public void Calculate_WhenPercentage_ReturnsCorrectValue()
    {
        var discount = new DiscountCalculatorCalculator(0.5, DiscountType.Percentage);
        var totalDiscount = discount.Calculate(2.0);
        totalDiscount.Should().Be(1.00);
    }
}