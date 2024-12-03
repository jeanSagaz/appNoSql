using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace appNoSql.Infra.Data.MongoDB.Interfaces
{
    public interface IMongoDbRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAll();

        Task<IEnumerable<TEntity>> GetFilter(Expression<Func<TEntity, bool>> filter);

        Task<TEntity> GetById(Guid id);

        Task Add(TEntity obj);

        Task AddMany(IEnumerable<TEntity> obj);

        Task Update(Guid id, TEntity obj);

        Task Remove(Guid id);

        Task Remove(Expression<Func<TEntity, bool>> filter);

        Task RemoveMany(Expression<Func<TEntity, bool>> filter);
    }
}
