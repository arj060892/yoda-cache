using Yoda.Cache.API.Models;
using Yoda.Cache.Entites;

namespace Yoda.Cache.API.Services
{
    public class UserService : IUserService
    {
        private readonly ICache<int, User> _cache;
        private readonly List<User> _users;

        public UserService(ICache<int, User> cache)
        {
            _cache = cache;

            _users = new List<User>
            {
                new User { Id = 1, Name = "User 1" },
                new User { Id = 2, Name = "User 2" }
            };
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            User user = await _cache.GetAsync(id);
            if (user == null)
            {
                user = _users.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    await _cache.PutAsync(id, user);
                }
            }
            return user;
        }

        public async Task AddUserAsync(User user)
        {
            _users.Add(user);
            await _cache.PutAsync(user.Id, user);
        }

        public async Task UpdateUserAsync(User user)
        {
            User existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Name = user.Name;
                await _cache.PutAsync(user.Id, user);
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            User user = _users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _users.Remove(user);
            }
        }
    }
}