using ElasticSearchTest.Models;

namespace ElasticSearchTest.Services
{
    public interface IElasticService
    {
        // create index
        Task<bool> CreateIndexIfNotExistsAsync(string indexName);


        // add or update user
        Task<bool> AddOrUpdateUser(User user);

        // add or update user bulk
        Task<bool> AddOrUpdateUserBulk(IEnumerable<User> users, string indexName);

        // get user
        Task<User> Get(string key);

        // search user
        Task<IEnumerable<User>> GetAll();


        // remove user
        Task<bool> RemoveUser(string key);

        // remove all users
        Task<long?> RemoveAllUsers();


    }
}
