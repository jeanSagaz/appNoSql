using Microsoft.Extensions.DependencyInjection;
using System;
using appNoSql.ConsoleApp.Configurations;
using appNoSql.Infra.Data.Redis.Interfaces;
using appNoSql.Domain.Models;
using appNoSql.Infra.Data.ElasticSearch.Interfaces;
using System.Threading.Tasks;
using appNoSql.Infra.Data.MongoDB.Interfaces;

namespace appNoSql.ConsoleApp
{
    class Program
    {
        private static IElasticSearchRepository<Person> _repositoryElasticSearch;
        private static IRedisRepository<Person> _genericRepositoryRedis;
        private static IPersonRepository _specializedRepositoryRedis;
        private static IMongoDbRepository<Person> _genericRepositoryMongoDB;

        static void Main(string[] args)
        {
            InitConfiguration();

            //MongoDB().Wait();
            //Redis().Wait();
            ElasticSearch().Wait();
            Console.ReadKey();
        }

        private static async Task Redis()
        {
            var person01 = new Person()
            {
                Id = new Guid("b8447a8c-06ca-4f20-88d7-9df27fcb5c5b"),
                Name = "Jean",
                Date = DateTime.Now
            };
            var resultGeneric = await _genericRepositoryRedis.GetById(person01.Id.ToString());

            if (resultGeneric is null)
            {
                Console.WriteLine($"01 - Adicionando no redis {person01.Id}, {person01.Name}");
                _genericRepositoryRedis.Set(person01.Id.ToString(), person01).Wait();
            }
            else
            {
                Console.WriteLine($"01 - Lendo do redis {resultGeneric.Id}, {resultGeneric.Name}");
            }

            var person02 = new Person()
            {
                Id = new Guid("9487cd0a-ac33-4eb1-b3ee-b75f807f3654"),
                Name = "Joan",
                Date = DateTime.Now
            };

            var resultSpecialized = await _specializedRepositoryRedis.GetById(person02.Id.ToString());
            if (resultSpecialized is null)
            {
                Console.WriteLine($"02 - Adicionando no redis {person02.Id}, {person02.Name}");
                await _specializedRepositoryRedis.Set(person02.Id.ToString(), person02);
            }
            else
            {
                Console.WriteLine($"02 - Lendo do redis {resultSpecialized.Id}, {resultSpecialized.Name}");
            }
        }

        private static async Task ElasticSearch()
        {
            var person01 = new Person()
            {
                Id = new Guid("b8447a8c-06ca-4f20-88d7-9df27fcb5c5b"),
                Name = "Jean",
                Date = DateTime.Now
            };
            await _repositoryElasticSearch.Add(person01);


            var people = await _repositoryElasticSearch.GetAll();
            foreach (var p in people)
            {
                Console.WriteLine($"Lendo do elasticsearch {p.Id}, {p.Name}");
            }

            var person = await _repositoryElasticSearch.GetById("b8447a8c-06ca-4f20-88d7-9df27fcb5c5b");
            if (person != null)
            {
                Console.WriteLine($"Busca: {person.Id}, {person.Name}");
            }

            var person02 = new Person()
            {
                Id = new Guid("b8447a8c-06ca-4f20-88d7-9df27fcb5c5b"),
                Name = "Candinha",
                Date = DateTime.Now
            };

            var updated = await _repositoryElasticSearch.Update(person02);
            if (updated.IsValid)
                Console.WriteLine($"Atualizado com sucesso");
            else
                Console.WriteLine($"Não atualizado");

            var removed = await _repositoryElasticSearch.Remove("b8447a8c-06ca-4f20-88d7-9df27fcb5c5b");
            if (removed.IsValid)
                Console.WriteLine($"Apagado com sucesso");
            else
                Console.WriteLine($"Não apagado");
        }

        private static async Task MongoDB()
        {
            var person = new Person()
            {
                Id = new Guid("f4af86bc-f3d0-42ba-ac5c-059dba578079"),
                Name = "Jan Ferreira",
                Date = DateTime.Now
            };            

            var result = await _genericRepositoryMongoDB.GetById(person.Id);
            if (result is not null)
            { 
                Console.WriteLine($"01 - Lendo do mongo {result.Id}, {result.Name}");
                person.Name = "Joan";
                await _genericRepositoryMongoDB.Update(person.Id, person);
            }
            else
            {
                await _genericRepositoryMongoDB.Add(person);
            }

            var all = await _genericRepositoryMongoDB.GetAll();
            foreach(var p in all)
            {
                Console.WriteLine($"01) Lendo do mongo {p.Id}, {p.Name}");
            }

            //await _genericRepositoryMongoDB.Remove(person.Id);
            await _genericRepositoryMongoDB.Remove(x => x.Id == person.Id);
        }

        private static void InitConfiguration()
        {
            var serviceProvider = new ServiceCollection();
            serviceProvider.ConfigureServices();
            var services = serviceProvider.BuildServiceProvider();

            _repositoryElasticSearch = services.GetService<IElasticSearchRepository<Person>>();
            _genericRepositoryRedis = services.GetService<IRedisRepository<Person>>();
            _specializedRepositoryRedis = services.GetService<IPersonRepository>();
            _genericRepositoryMongoDB = services.GetService<IMongoDbRepository<Person>>();
        }
    }
}
