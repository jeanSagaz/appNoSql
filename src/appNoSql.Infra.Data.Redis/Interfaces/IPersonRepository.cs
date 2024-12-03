using appNoSql.Domain.Models;

namespace appNoSql.Infra.Data.Redis.Interfaces
{
    public interface IPersonRepository : IRedisRepository<Person>
    {
    }
}
