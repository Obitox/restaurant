using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Infrastructure.Cache
{
    public interface IRedis
    {
        HashEntry[] GetHash(RedisKey key);
        RedisValue GetString(RedisKey key);
        void SetHash(RedisKey key, HashEntry[] hashFields);
        bool SetString(RedisKey key, RedisValue value);
        HashEntry[] GetAllHash(RedisKey key);
        bool DelHash(RedisKey key);
        string GetHashField(RedisKey key, RedisValue field);
    }
}
