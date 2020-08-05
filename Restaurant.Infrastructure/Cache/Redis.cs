using Microsoft.Extensions.Options;
using Restaurant.Infrastructure.Helpers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Redis
{
    public class Redis : Cache.IRedis
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public Redis(IConnectionMultiplexer redis)
        {
            _redis = redis;
            if(_redis.IsConnected)
                _db = _redis.GetDatabase();
            
        }

        public void SetHash(RedisKey key, HashEntry[] hashFields) => _db.HashSet(key, hashFields);

        public bool SetString(RedisKey key, RedisValue value) => _db.StringSet(key, value);

        public HashEntry[] GetHash(RedisKey key) => _db.HashGetAll(key);

        public RedisValue GetString(RedisKey key) => _db.StringGet(key);

        public HashEntry[] GetAllHash(RedisKey key) => _db.HashGetAll(key);

        public bool DelHash(RedisKey key) => _db.KeyDelete(key);

        public string GetHashField(RedisKey key, RedisValue field) => _db.HashGet(key, field);
    }
}
