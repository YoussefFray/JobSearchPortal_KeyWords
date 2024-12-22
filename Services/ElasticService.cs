using JobSearchPortal_KeyWords.Models;
using JobSearchPortal_KeyWords.Configuration;
using Microsoft.Extensions.Options;
using Nest;

namespace JobSearchPortal_KeyWords.Services
{
    public class ElasticService : IElasticService
    {
        private readonly ElasticClient _client;
        private readonly ElasticSettings _elasticSettings;

        public ElasticService(IOptions<ElasticSettings> optionsMonitor)
        {
            _elasticSettings = optionsMonitor.Value;

            
            var settings = new ConnectionSettings(new Uri(_elasticSettings.Url))
                .DefaultIndex(_elasticSettings.DefaultIndex);

            
            // settings.BasicAuthentication(_elasticSettings.UserName, _elasticSettings.Password);

            _client = new ElasticClient(settings);
        }

        public async Task<bool> AddOrUpdate(User user)
        {
            var response = await _client.IndexDocumentAsync(user);
            return response.IsValid;
        }

        public async Task<bool> AddOrUpdateBulk(IEnumerable<User> users, string indexName)
        {
            var response = await _client.BulkAsync(b => b
                .Index(indexName)
                .IndexMany(users)
            );
            return response.IsValid;
        }

        public async Task CreateIndexIfNotExistsAsync(string indexName)
        {
            var existsResponse = await _client.Indices.ExistsAsync(indexName);
            if (!existsResponse.Exists)
            {
                var createResponse = await _client.Indices.CreateAsync(indexName, c => c
                    .Map<User>(m => m.AutoMap())
                );
                if (!createResponse.IsValid)
                {
                    throw new Exception($"Failed to create index: {createResponse.ServerError?.Error.Reason}");
                }
            }
        }

        public async Task<User?> Get(string id)
        {
            var response = await _client.GetAsync<User>(id, idx => idx.Index(_elasticSettings.DefaultIndex));
            return response.Found ? response.Source : null;
        }

        public async Task<List<User>?> GetAll()
        {
            var response = await _client.SearchAsync<User>(s => s
                .Index(_elasticSettings.DefaultIndex)
                .Query(q => q.MatchAll())
            );
            return response.IsValid ? response.Documents.ToList() : null;
        }

        public async Task<bool> Remove(string id)
        {
            var response = await _client.DeleteAsync<User>(id, idx => idx.Index(_elasticSettings.DefaultIndex));
            return response.IsValid;
        }

        public async Task<bool> RemoveAll()
        {
            var response = await _client.DeleteByQueryAsync<User>(q => q
                .Index(_elasticSettings.DefaultIndex)
                .Query(q => q.MatchAll())
            );
            return response.IsValid;
        }

        Task<bool> IElasticService.Get(string id)
        {
            throw new NotImplementedException();
        }
    }
}
