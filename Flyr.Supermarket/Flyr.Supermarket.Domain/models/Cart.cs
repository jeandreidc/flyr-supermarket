namespace Flyr.Supermarket.Domain.models;

public class Cart()
{
    private readonly Dictionary<string, int> _cart = new();

    private Cart(Dictionary<string, int> cartDictionary) : this()
    {
        _cart = cartDictionary;
    }
    
    public void Add(Product product)
    {
        if (!_cart.TryAdd(product.ProductCode, 1))
        {
            _cart[product.ProductCode] += 1;
        }
    }

    public int this[string code] => _cart.GetValueOrDefault(code, 0);
    public bool Contains(Dictionary<string, int> itemConditions)
    {
        return itemConditions.All(kv => _cart.ContainsKey(kv.Key) && _cart[kv.Key] >= kv.Value);
    }

    public Cart Clone()
    {
        return (Cart)MemberwiseClone();
    }
    
    public Cart Remove(Dictionary<string, int> itemsToSubtract)
    {
        var cartCopy = new Dictionary<string, int>(_cart);
        foreach (var key in itemsToSubtract.Keys)
        {
            cartCopy[key] -= itemsToSubtract[key];
            if(cartCopy[key] == 0) cartCopy.Remove(key);
        }

        return new Cart(cartCopy);
    }

    public Cart Merge(Dictionary<string, int> lineItems)
    {
        var cartCopy = new Dictionary<string, int>(_cart);
        foreach (var key in lineItems.Keys.Where(key => !cartCopy.TryAdd(key, lineItems[key])))
        {
            cartCopy[key] += lineItems[key];
        }
        
        return new Cart(cartCopy);
    }

    public double ComputeTotalPrice(Dictionary<string, double> priceDictionary) =>
        _cart.Sum(kv => priceDictionary[kv.Key] * kv.Value);

    public Dictionary<string, int> AsDictionary()
    {
        return new Dictionary<string, int>(_cart);
    }
}