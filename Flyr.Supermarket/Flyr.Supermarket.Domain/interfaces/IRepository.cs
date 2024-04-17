using System.Linq.Expressions;
using Flyr.Supermarket.Domain.models;

namespace Flyr.Supermarket.Domain.interfaces;

public interface IRepository<TEntity> where TEntity : IDomainModel
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>>? filter = null);
    Task<TEntity?> GetByIdAsync(string id);
    Task InsertAsync(TEntity entity);

}