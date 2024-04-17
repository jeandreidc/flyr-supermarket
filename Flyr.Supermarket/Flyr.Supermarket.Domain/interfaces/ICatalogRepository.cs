using Flyr.Supermarket.Domain.models;

namespace Flyr.Supermarket.Domain.interfaces;

public interface ICatalogRepository : IRepository<Product>
{
    Dictionary<string, double> GetPriceDictionary();
}