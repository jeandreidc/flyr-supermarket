using Flyr.Supermarket.Domain.models;
using Flyr.Supermarket.Domain.models.PricingRules;

namespace Flyr.Supermarket.Domain.exceptions;

public class InvalidDiscountException(DiscountType discountType) : Exception($"Discount type: {discountType} is not handled")
{
    
}