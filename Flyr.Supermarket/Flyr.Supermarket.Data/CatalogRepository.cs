using System.Linq.Expressions;
using Flyr.Supermarket.Domain.exceptions;
using Flyr.Supermarket.Domain.interfaces;
using Flyr.Supermarket.Domain.models;

namespace Flyr.Supermarket.Data;

public class CatalogRepository : ICatalogRepository
{
    private static Dictionary<string, double> _prices = new()
    {
        ["GR1"] = 3.11,
        ["SR1"] = 5.00,
        ["CF1"] = 11.23
    };

    private static List<Product> _catalog =
    [
        new Product("GR1", "Green tea", 3.11, "GBP"),
        new Product("SR1", "Strawberries", 5.00, "GBP"),
        new Product("CF1", "Coffee", 11.23, "GBP")
    ];

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult(_catalog.AsEnumerable());
    }

    public Task<IEnumerable<Product>> FindAllAsync(Func<Product, bool> filter)
    {
        return Task.FromResult(_catalog.Where(filter));
    }

    public Task<Product?> GetByIdAsync(string code)
    {
        return Task.FromResult(_catalog.FirstOrDefault(p => p.ProductCode == code));
    }

    public Task InsertAsync(Product entity)
    {
        if (_catalog.Any(p => p.ProductCode == entity.ProductCode))
            throw new InvalidProductException(entity.ProductCode);
        
        _catalog.Add(entity);
        return Task.CompletedTask;
    }

    public Dictionary<string, double> GetPriceDictionary()
    {
        return _prices;
    }
}