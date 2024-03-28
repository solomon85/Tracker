using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker
{
    public class Redis
    {
        public static IDatabase redis;
        public static void Configure()
        {
            var configuration = ConfigurationOptions.Parse("localhost:6379");
            var redisConnection = ConnectionMultiplexer.Connect(configuration);
            redis = redisConnection.GetDatabase();
        }

        public static void SetCacheData(string key, string value)
        {
            redis.StringSet(key, value);
        }

        public static bool KeyDelete(string key) => redis.KeyDelete(key);
        public static String GetCacheData(string key) => redis.StringGet(key);
        public static T GetCacheData<T>(string key) => (T)Convert.ChangeType(redis.StringGet(key), typeof(T));
        public static bool KeyExpire(string key, DateTime? date) => redis.KeyExpire(key, date);

    }
}
