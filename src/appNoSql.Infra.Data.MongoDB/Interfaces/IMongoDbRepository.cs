using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace appNoSql.Infra.Data.MongoDB.Interfaces
{
    public interface IMongoDbRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetFilterAsync(Expression<Func<TEntity, bool>> filter);

        Task<TEntity> GetByIdAsync(Guid id);

        Task AddAsync(TEntity obj);

        Task AddManyAsync(IEnumerable<TEntity> obj);

        Task<bool> UpdateAsync(Guid id, TEntity obj);

        Task<ReplaceOneResult> UpdateAsync(TEntity obj);

        Task<DeleteResult> RemoveAsync(Guid id);

        Task<DeleteResult> RemoveAsync(Expression<Func<TEntity, bool>> filter);

        Task<DeleteResult> RemoveManyAsync(Expression<Func<TEntity, bool>> filter);
    }
}
