using appNoSql.Domain.Core.Models;
using Nest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace appNoSql.Infra.Data.ElasticSearch.Interfaces
{
    public interface IElasticSearchRepository<T> where T : Entity
    {
        Task<IEnumerable<T>> GetAll();

        Task<T> GetById(string id);

        Task<IndexResponse> Add(T obj);

        Task<BulkResponse> Add(IEnumerable<T> obj);

        Task<UpdateResponse<T>> Update(T obj);

        Task<DeleteResponse> Remove(string id);
    }
}
