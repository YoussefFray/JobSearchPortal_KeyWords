using System.Globalization;
using JobSearchPortal_KeyWords.Models;

namespace JobSearchPortal_KeyWords.Services
{
    public interface IElasticService
    {
        Task CreateIndexIfNotExistsAsync(String indexName);
        Task<bool> AddOrUpdate(User user);

        Task<bool> AddOrUpdateBulk(IEnumerable<User> users,string indexName);

        Task<bool> Get(string id);   

        Task<List<User>?> GetAll();

        Task<bool> Remove(string id);

        Task<bool> RemoveAll();

    }
}
