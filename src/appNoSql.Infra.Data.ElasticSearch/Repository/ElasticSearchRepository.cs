using appNoSql.Domain.Core.Models;
using appNoSql.Infra.Data.ElasticSearch.Interfaces;
using Microsoft.Extensions.Configuration;
using Nest;

namespace appNoSql.Infra.Data.ElasticSearch.Repository
{
    public class ElasticSearchRepository<T> : IElasticSearchRepository<T> where T : Entity
    {
        private readonly IElasticClient _elasticClient;
        public string IndexName { get; }

        public ElasticSearchRepository(IElasticClient elasticClient,
            IConfiguration configuration)
        {
            _elasticClient = elasticClient;
            IndexName = configuration["ElasticsearchSettings:defaultIndex"] ?? throw new ArgumentNullException(nameof(configuration));
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            var search = new SearchDescriptor<T>(IndexName).MatchAll();
            var response = await _elasticClient.SearchAsync<T>(search);

            return response.Hits.Select(hit => hit.Source).ToList();
        }

        public virtual async Task<T?> GetById(string id)
        {
            var response = await _elasticClient.GetAsync(DocumentPath<T>.Id(id).Index(IndexName));

            if (response.IsValid)
                return response.Source;

            return null;
        }

        public virtual async Task<IndexResponse> Add(T obj) =>        
            await _elasticClient.IndexAsync(obj, descriptor => descriptor.Index(IndexName));

        public virtual async Task<BulkResponse> Add(IEnumerable<T> obj) =>        
            await _elasticClient.IndexManyAsync(obj, IndexName);        

        public virtual async Task<UpdateResponse<T>> Update(T obj) =>
            await _elasticClient.UpdateAsync<T>(DocumentPath<T>.Id(obj.Id).Index(IndexName), p => p.Doc(obj));

        public virtual async Task<DeleteResponse> Remove(string id) => 
            await _elasticClient.DeleteAsync<T>(DocumentPath<T>.Id(id).Index(IndexName));
    }
}
