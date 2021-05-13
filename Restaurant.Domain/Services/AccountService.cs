using System;
using Restaurant.Domain.Helpers;
using StackExchange.Redis;
using System.Threading.Tasks;
using Restaurant.DAL.MySQL.Models;
using Restaurant.DAL.MySQL.Repository;

namespace Restaurant.Domain.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserRepository _userRepository;
        private readonly IDatabase _db;

        public AccountService(UserRepository userRepository, IConnectionMultiplexer redis)
        {
            _userRepository = userRepository ?? throw new NotImplementedException("User repository instance not implemented");
            if (redis == null) throw new NotImplementedException("Redis instance not found");
            if (redis.IsConnected)
                _db = redis.GetDatabase();
        }

        public User Profile(string userId)
        {
            var isParsed = ulong.TryParse(userId, out var id);
            if (!isParsed) return null;
            
            var user = _userRepository.Single(u => u.UserId == id);
            return user.Profile();

        }

        public async Task<bool> ValidateEmailConfirmation(string userId, string confirmToken)
        {
            string token = _db.StringGet(userId);

            if (string.IsNullOrEmpty(token)) return false;
            if (!token.Equals(confirmToken)) return false;
            
            var isParsed = ulong.TryParse(userId, out var id);
            if (!isParsed) return false;
            
            var isUpdated = await _userRepository.UpdateUserEmailConfirmedByEntityId(id);
            return isUpdated;

        }
    }
}
