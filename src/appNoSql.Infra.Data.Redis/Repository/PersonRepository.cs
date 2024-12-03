using appNoSql.Domain.Models;
using appNoSql.Infra.Data.Redis.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace appNoSql.Infra.Data.Redis.Repository
{
    public class PersonRepository : RedisRepository<Person>, IPersonRepository
    {
        public PersonRepository(IDistributedCache cache) : base(cache)
        {

        }
    }
}
