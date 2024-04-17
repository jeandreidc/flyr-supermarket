using Flyr.Supermarket.Domain.models;

namespace Flyr.Supermarket.Domain.interfaces;

public interface ICheckout
{
    void Scan(Product product);
    void Scan(string productCode);
    double Total();
}