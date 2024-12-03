using appNoSql.Domain.Core.Models;
using System;
using System.Threading.Tasks;

namespace appNoSql.Infra.Data.Redis.Interfaces
{
    public interface IRedisRepository<T> where T : Entity
    {
        Task Set(string id, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null);
        Task<bool> Update(string id, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null);
        Task<bool> Remove(string id);
        Task<T> GetById(string id);
    }
}
