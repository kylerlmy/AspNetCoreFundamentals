using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackExchangeRedisExample.Services
{
    public interface IRepository
    {
        Task<Person> GetAsync(string id);
        IEnumerable<string> GetUsers();
        Task<Person> UpdatAsync(Person value);
        Task<bool> DeleteAsync(string id);
        Task AddNewPerson(Person person);
    }

    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class PersonRepository : IRepository
    {
        private readonly ILogger<PersonRepository> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public PersonRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
        {
            _logger = loggerFactory.CreateLogger<PersonRepository>();
            _redis = redis;
            _database = _redis.GetDatabase();
        }

        public Task AddNewPerson(Person person)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }

        public async Task<Person> GetAsync(string id)
        {
            var data = await _database.StringGetAsync(id);
            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Person>(data);
        }

        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();
            var data = server.Keys();

            return data?.Select(k => k.ToString());
        }

        public async Task<Person> UpdatAsync(Person value)
        {
            var created = await _database.StringSetAsync(value.Id, JsonConvert.SerializeObject(value));
            if (!created)
            {
                _logger.LogInformation("Problem occur persisting the item.");
                return null;
            }

            _logger.LogInformation("Basket item persisted succesfully.");

            return await GetAsync(value.Id);
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();

            return _redis.GetServer(endpoint.First());
        }
    }

}
