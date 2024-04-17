using Flyr.Supermarket.Domain;
using Flyr.Supermarket.Domain.models.PricingRules;

var pricing_rules = new List<IPricingRule>
{
    new PricingRule(new Dictionary<string, PromoCondition> {["GR1"] = new(2, 2)}, new DiscountCalculatorCalculator(0.5, DiscountType.Percentage)),
    new PricingRule(new Dictionary<string, PromoCondition> {["CF1"] = new(3, null)}, new DiscountCalculatorCalculator(1.0/3.0, DiscountType.Percentage)),
    new PricingRule(new Dictionary<string, PromoCondition> {["SR1"] = new(3, null)}, new DiscountCalculatorCalculator(.50/5.00, DiscountType.Percentage))
};

var co = new Checkout(pricing_rules);
co.Scan("GR1");
co.Scan("CF1");
co.Scan("SR1");
Console.WriteLine(co.Total());
