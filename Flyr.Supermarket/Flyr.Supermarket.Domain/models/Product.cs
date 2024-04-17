namespace Flyr.Supermarket.Domain.models;

public class Product(string productCode, string productName, double price, string currency) : IDomainModel
{
    public string ProductCode { get; } = productCode;
    public string ProductName { get; } = productName;
    public double Price { get; } = price;
    public string Currency { get; } = currency;
    public string Id { get; }
}