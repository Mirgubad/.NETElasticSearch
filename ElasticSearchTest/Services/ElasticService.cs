using Elastic.Clients.Elasticsearch;
using ElasticSearchTest.Configuration;
using ElasticSearchTest.Models;
using Microsoft.Extensions.Options;

namespace ElasticSearchTest.Services
{
    public class ElasticService : IElasticService
    {
        private readonly ElasticsearchClient _client;
        private readonly ElasticSettings _settings;


        public ElasticService(IOptions<ElasticSettings> optionsMonitor)
        {
            _settings = optionsMonitor.Value;

            var settings = new ElasticsearchClientSettings(
                new Uri(_settings.Url))
                //.Authentication()
                .DefaultIndex(_settings.DefaultIndex);

            _client = new ElasticsearchClient(settings);
        }



        public async Task<bool> AddOrUpdateUser(User user)
        {
            var response = await _client.IndexAsync(user, idx =>
            {
                idx.Index(_settings.DefaultIndex)
                .OpType(OpType.Index);
            });
            return response.IsValidResponse;
        }

        public async Task<bool> AddOrUpdateUserBulk(IEnumerable<User> users, string indexName)
        {
            var response = await _client.BulkAsync(b =>
            {
                b.Index(_settings.DefaultIndex)
                .UpdateMany(users, (ud, u) =>
                {
                    ud.Doc(u).DocAsUpsert(true);
                });
            });
            return response.IsValidResponse;
        }

        public async Task<bool> CreateIndexIfNotExistsAsync(string indexName)
        {
            if (!_client.Indices.Exists(indexName).Exists)
                await _client.Indices.CreateAsync(indexName);

            return true;
        }

        public async Task<User> Get(string key)
        {
            var response = await _client.GetAsync<User>(key, g =>
            {
                g.Index(_settings.DefaultIndex);
            });
            return response.Source;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var response = await _client.SearchAsync<User>(s =>
            {
                s.Index(_settings.DefaultIndex);
            });

            return response.Documents.ToList();
        }

        public async Task<long?> RemoveAllUsers()
        {
            var response = await _client.DeleteByQueryAsync<User>(d =>
            {
                d.Indices(_settings.DefaultIndex);
            });

            return response.IsValidResponse ? response.Deleted : default;
        }


        public async Task<bool> RemoveUser(string key)
        {
            var response = await _client.DeleteAsync<User>(key, d =>
            {
                d.Index(_settings.DefaultIndex);
            });

            return response.IsValidResponse;
        }
    }
}
