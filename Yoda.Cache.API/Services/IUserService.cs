using Yoda.Cache.API.Models;

namespace Yoda.Cache.API.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);

        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(int id);
    }
}