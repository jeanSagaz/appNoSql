using appNoSql.Domain.Core.Configurations;
using appNoSql.Infra.Data.MongoDB.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using appNoSql.Infra.Data.MongoDB.Context;

namespace appNoSql.Infra.Data.MongoDB.Repository
{
    public class MongoDbRepository<TEntity> : MongoDbContext, IMongoDbRepository<TEntity> where TEntity : class
    {
        private IMongoCollection<TEntity> DbSet;

        public MongoDbRepository(MongoDbConfiguration configurations) : base(configurations)
        {
            DbSet = GetCollection<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            var all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
            return await all.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetFilter(Expression<Func<TEntity, bool>> filter)
        {
            var all = await DbSet.FindAsync(filter);
            return await all.ToListAsync();
        }

        public virtual async Task<TEntity> GetById(Guid id)
        {
            //var objectId = new ObjectId(id);
            var data = await DbSet.FindAsync(Builders<TEntity>.Filter.Eq("_id", id));
            return await data.FirstOrDefaultAsync();
        }        

        public virtual async Task Add(TEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException($"{typeof(TEntity).Name} object is null");
            }

            await DbSet.InsertOneAsync(obj);
        }

        public virtual async Task AddMany(IEnumerable<TEntity> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException($"{typeof(TEntity).Name} object is null");
            }

            await DbSet.InsertManyAsync(obj);
        }

        public virtual async Task Update(Guid id, TEntity obj)
        {
            var result = await this.GetById(id);
            if (result is null)
            {
                throw new ArgumentException($"{typeof(TEntity).Name} get by id is null");
            }

            await DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", id), obj);
        }

        public virtual async Task Remove(Guid id) => await DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id));

        public virtual async Task Remove(Expression<Func<TEntity, bool>> filter) => await DbSet.DeleteOneAsync(filter);

        public virtual async Task RemoveMany(Expression<Func<TEntity, bool>> filter) => await DbSet.DeleteManyAsync(filter);

        public void Dispose() => GC.SuppressFinalize(this);
    }
}
