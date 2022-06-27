using appNoSql.Domain.Core.Models;
using appNoSql.Infra.Data.ElasticSearch.Interfaces;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appNoSql.Infra.Data.ElasticSearch.Repository
{
    public class RepositoryElasticSearch<T> : IRepositoryElasticSearch<T> where T : Entity
    {
        private readonly IElasticClient _elasticClient;

        public RepositoryElasticSearch(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        //public virtual async Task<ICollection<T>> GetAll()
        public virtual async Task<IEnumerable<T>> GetAll()
        {
            var results = new List<T>();
            var isScrollSetHasData = true;

            //var result = _elasticClient.Search<T>(s => s
            //    .Index(nameof(T).ToLower())
            //    .Sort(q => q.Descending(p => p.Id)))?.Documents;
            //var result = (await _elasticClient.SearchAsync<T>(s => s
            //    .Index(nameof(T).ToLower())
            //    .Sort(q => q.Descending(p => p.Id))))?.Documents;
            var result = (await _elasticClient.SearchAsync<T>(//s => s
                //.Index(nameof(T).ToLower())
                //.Sort(q => q.Descending(p => p.Id)))
                ))?.Documents;

            var result2 = _elasticClient.Search<T>(s => s
                .Index(nameof(T).ToLower())
                .MatchAll()).Documents.ToList();

            var result3 = _elasticClient.Search<T>(s => s
                .Index(nameof(T).ToLower())
                .From(0)
                .Size(10)
                .MatchAll()).Documents.ToList();

            //scroll
            var result4 = _elasticClient.Search<T>(s => s
                .Index(nameof(T).ToLower())
                .From(0)
                .Size(10)
                .Scroll("1m")
                .MatchAll());

            if (result4.Documents.Any())
                results.AddRange(result4.Documents);

            var scrollid = result4.ScrollId;

            while (isScrollSetHasData)
            {
                var loopingResponse = _elasticClient.Scroll<T>("1m", scrollid);
                if (loopingResponse.IsValid)
                {
                    results.AddRange(loopingResponse.Documents);
                    scrollid = loopingResponse.ScrollId;
                }
                isScrollSetHasData = loopingResponse.Documents.Any();
            }

            _elasticClient.ClearScroll(new ClearScrollRequest(scrollid));

            return result.ToList();
        }

        //public virtual async Task<ICollection<T>> GetById(string id)
        public virtual async Task<T> GetById(string id)
        {
            //usado em lowcase
            var query = new QueryContainerDescriptor<T>().Term(t => t.Field(f => f.Id).Value(id));

            //var result = _elasticClient.Search<T>(s => s
            //    .Index(nameof(T).ToLower())
            //    .Query(s => query)
            //    .Size(10)
            //    .Sort(q => q.Descending(p => p.Id)))?.Documents;
            var result = (await _elasticClient.SearchAsync<T>(s => s
                //.Index(nameof(T).ToLower())
                .Query(s => query)
                .Size(10)
                //.Sort(q => q.Descending(p => p.Id))
                ))?.Documents;

            var result2 = _elasticClient.Search<T>(s => s
                //.Index(nameof(T).ToLower())
                .Query(s => s.Wildcard(w => w.Field(f => f.Id).Value(id + "*")))
                .Size(10)
                //.Sort(q => q.Descending(p => p.Id))
                )?.Documents;

            var result3 = _elasticClient.Search<T>(s => s
                //.Index(nameof(T).ToLower())
                .Query(s => s.Match(m => m.Field(f => f.Id).Query(id))) //Procura cada termo com o operador OR, case insensitive
                                                                        //.Query(s => s.Match(m => m.Field(f => f.Name).Query(name).Operator(Operator.And)) //com o operador AND
                .Size(10)
                //.Sort(q => q.Descending(p => p.Id))
                )?.Documents;

            var result4 = (await _elasticClient.SearchAsync<T>(s => s
                //.Index(nameof(T).ToLower())
                .Query(s => s.MatchPhrase(m => m.Field(f => f.Id).Query(id))) //Procura o termo que contenha a frase exata
                                                                              //.Query(s => s.MatchPhrase(m => m.Field(f => f.Name).Query(name).Slop(1))) //Procura o termo que contenha a frase exata, pulando uma inconsistencia
                .Size(10)
                .Sort(q => q.Descending(p => p.Date))
                ))?.Documents;

            //return result?.ToList();
            return result4.FirstOrDefault();            
        }

        public virtual async Task<IndexResponse> Add(T obj)
        {
            //var descriptor = new BulkDescriptor();

            //if (!_elasticClient.Indices.Exists(nameof(obj).ToLower()).Exists)
            //    await _elasticClient.Indices.CreateAsync(nameof(obj).ToLower());

            //_elasticClient.IndexMany<obj>(obj.GetSampleData());

            //or
            //descriptor.UpdateMany<obj>(obj.GetSampleData(), (b, u) => b
            //    .Index(nameof(obj).ToLower())
            //    .Doc(u)
            //    .DocAsUpsert());            

            //var insert = _elasticClient.Bulk(descriptor);

            //var insert = await _elasticClient.IndexDocumentAsync(obj);

            //if (!insert.IsValid)
            //    throw new Exception(insert.OriginalException.ToString());

            //return insert;

            return await _elasticClient.IndexDocumentAsync(obj);
        }

        public virtual async Task<BulkResponse> Add(IEnumerable<T> obj)
        {
            //var result = await _elasticClient.IndexManyAsync(obj);
            //if (result.Errors)
            //{
            //    // the response can be inspected for errors
            //    foreach (var itemWithError in result.ItemsWithErrors)
            //    {
            //        Console.WriteLine("Failed to index document {0}: {1}",itemWithError.Id, itemWithError.Error);
            //    }
            //}

            return await _elasticClient.IndexManyAsync(obj);
        }

        public virtual async Task<UpdateResponse<T>> Update(T obj)
        {
            return await _elasticClient.UpdateAsync<T>(obj.Id, x => x.Doc(obj));
        }        

        public virtual async Task<DeleteResponse> Remove(string id)
        {
            return await _elasticClient.DeleteAsync<T>(id);
        }
    }
}
