using appNoSql.Domain.Core.Configurations;
using appNoSql.Infra.Data.MongoDB.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;
using appNoSql.Infra.Data.MongoDB.Context;
using appNoSql.Domain.Core.Models;

namespace appNoSql.Infra.Data.MongoDB.Repository
{
    public class MongoDbRepository<TEntity> : MongoDbContext, IMongoDbRepository<TEntity> where TEntity : Entity
    {
        private IMongoCollection<TEntity> DbSet;

        public MongoDbRepository(MongoDbConfiguration configurations) : base(configurations)
        {
            DbSet = GetCollection<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
            return await all.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetFilterAsync(Expression<Func<TEntity, bool>> filter)
        {
            var all = await DbSet.FindAsync(filter);
            return await all.ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            //var objectId = new ObjectId(id);
            var data = await DbSet.FindAsync(Builders<TEntity>.Filter.Eq("_id", id));
            return await data.FirstOrDefaultAsync();
        }        

        public virtual async Task AddAsync(TEntity obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException($"{typeof(TEntity).Name} object is null");
            }

            await DbSet.InsertOneAsync(obj);
        }

        public virtual async Task AddManyAsync(IEnumerable<TEntity> obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException($"{typeof(TEntity).Name} object is null");
            }

            await DbSet.InsertManyAsync(obj);
        }

        public virtual async Task<bool> UpdateAsync(Guid id, TEntity obj)
        {
            var result = await this.GetByIdAsync(id);
            if (result is null)
            {
                return false;
            }

            await DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", id), obj);

            return true;
        }

        public virtual async Task<ReplaceOneResult> UpdateAsync(TEntity obj) =>        
            // await DbSet.ReplaceOneAsync(it => it.Id == obj.Id, obj);
            await DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.Id), obj);        

        public virtual async Task<DeleteResult> RemoveAsync(Guid id) => 
            await DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id));

        public virtual async Task<DeleteResult> RemoveAsync(Expression<Func<TEntity, bool>> filter) => 
            await DbSet.DeleteOneAsync(filter);

        public virtual async Task<DeleteResult> RemoveManyAsync(Expression<Func<TEntity, bool>> filter) => 
            await DbSet.DeleteManyAsync(filter);

        public void Dispose() => GC.SuppressFinalize(this);
    }
}
