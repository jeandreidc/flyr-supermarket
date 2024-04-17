using System.Linq.Expressions;
using Flyr.Supermarket.Domain.interfaces;
using Flyr.Supermarket.Domain.models;

namespace Flyr.Supermarket.Data;

public class CatalogRepository : ICatalogRepository
{
    private readonly Dictionary<string, double> _catalog = new()
    {
        ["GR1"] = 3.11,
        ["SR1"] = 5.00,
        ["CF1"] = 11.23
    };
    public Task<IEnumerable<Product>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Product>> FindAllAsync(Expression<Func<Product, bool>>? filter = null)
    {
        throw new NotImplementedException();
    }

    public Task<Product> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, double> GetPriceDictionary()
    {
        throw new NotImplementedException();
    }
}