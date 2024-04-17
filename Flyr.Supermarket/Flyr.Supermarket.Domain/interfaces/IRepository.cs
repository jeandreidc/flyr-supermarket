using System.Linq.Expressions;
using Flyr.Supermarket.Domain.models;

namespace Flyr.Supermarket.Domain.interfaces;

public interface IRepository<TEntity> where TEntity : IDomainModel
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAllAsync(Func<TEntity, bool> filter);
    Task<Product?> GetByIdAsync(string code);
    Task InsertAsync(TEntity entity);

}