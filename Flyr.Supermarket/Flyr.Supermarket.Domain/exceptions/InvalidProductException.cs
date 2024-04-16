namespace Flyr.Supermarket.Domain.exceptions;

public class InvalidProductException(string itemCode) : Exception($"Item code: {itemCode} does not exist in the catalog")
{
    
}