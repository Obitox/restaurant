using Restaurant.Infrastructure.Helpers;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public AccountService(IUserRepository userRepository, IConnectionMultiplexer redis)
        {
            _userRepository = userRepository;
            _redis = redis;
            if (_redis.IsConnected)
                _db = _redis.GetDatabase();
        }

        public User Profile(string userId)
        {
            bool isParsed = ulong.TryParse(userId, out ulong id);
            User user = null;

            if(isParsed)
            {
                user = _userRepository.Single(u => u.UserId == id);
                return user.Profile();
            }

            return null;
        }

        public async Task<bool> ValidateEmailConfirmation(string userId, string confirmToken)
        {
            string token = _db.StringGet(userId);

            if (!string.IsNullOrEmpty(token))
                if (token.Equals(confirmToken))
                {
                    bool isParsed = ulong.TryParse(userId, out ulong id);

                    if (isParsed)
                    {
                        bool isUpdated = await _userRepository.UpdateUserEmailConfirmedByEntityId(id);
                        return isUpdated;
                    }

                    return false;
                }

            return false;
        }
    }
}
