using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace Solitude.Exchange.Core.Redis
{
    #region Redis连接类
    public class RedisClient : IDisposable
    {

        private IConfigurationRoot _config;
        private ConcurrentDictionary<string, ConnectionMultiplexer> _connections;

        public RedisClient(IConfigurationRoot config)
        {
            _config = config;
            _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        }

        private ConnectionMultiplexer GetConnect(IConfigurationSection redisConfig)
        {
            var redisInstanceName = redisConfig["InstanceName"];
            var connStr = redisConfig["Conn"];
            return _connections.GetOrAdd(redisInstanceName, p => ConnectionMultiplexer.Connect(connStr));
        }

        /// <summary>
        /// 检查入参数
        /// </summary>
        /// <param name="configName">RedisConfig配置文件中的 Redis_Default/Redis_6 名称</param>
        /// <returns></returns>
        private IConfigurationSection CheckeConfig(string configName)
        {
            IConfigurationSection redisConfig =  _config.GetSection("RedisConfig").GetSection(configName);
            if (redisConfig == null)
            {
                throw new ArgumentNullException($"{configName}找不到对应的RedisConfig配置！");
            }
            var redisInstanceName = redisConfig["InstanceName"];
            var connStr = redisConfig["Conn"];
            if (string.IsNullOrEmpty(redisInstanceName))
            {
                throw new ArgumentNullException($"{configName}找不到对应的InstanceName");
            }
            if (string.IsNullOrEmpty(connStr))
            {
                throw new ArgumentNullException($"{configName}找不到对应的Connection");
            }
            return redisConfig;
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="db">默认为0：优先代码的db配置，其次config中的配置</param>
        /// <returns></returns>
        public IDatabase GetDatabase(string configName = "default")
        {
            IConfigurationSection redisConfig = CheckeConfig(configName);
            return GetConnect(redisConfig).GetDatabase();
        }

        public IServer GetServer(string configName = null, int endPointsIndex = 0)
        {
            IConfigurationSection redisConfig = CheckeConfig(configName);
            var connStr = redisConfig["Connection"];

            var confOption = ConfigurationOptions.Parse((string)connStr);
            return GetConnect(redisConfig).GetServer(confOption.EndPoints[endPointsIndex]);
        }

        public ISubscriber GetSubscriber(string configName = null)
        {
            IConfigurationSection redisConfig = CheckeConfig(configName);
            return GetConnect(redisConfig).GetSubscriber();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
    #endregion Redis连接类

    #region Redis操作类
    public class RedisCore
    {
        private static   RedisClient  _redisClient;
        private static object _lockObj = new object();

        /// <summary>
        /// 实例化Redis客户端
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private static void InitClient(IConfigurationRoot config = null)
        {
            if (config == null)
            {
                config = BaseCore.Configuration;
            }
            if (_redisClient == null)
            {
                lock (_lockObj)
                {
                    if (_redisClient == null)
                    {
                        _redisClient = new RedisClient(config);
                    }
                }
            }          
        }

        public static RedisClient GetRedisClient(IConfigurationRoot config = null)
        {
                InitClient(config);
                return _redisClient;            
        }
    }

    public class RedisHelper
    {
        private static RedisClient _redisClient;
        private static readonly string RedisLockKey = "LockKey";
        private static readonly string RedisLockToken = "LockToken";

        static RedisHelper()
        {
            _redisClient= RedisCore.GetRedisClient();
            Db = _redisClient.GetDatabase();
        }

        public static IDatabase Db { get; private set; }

        /// <summary>
        /// 开启redis事务锁
        /// </summary>
        /// <param name="lockKey"></param>
        /// <param name="lockToken"></param>
        /// <returns></returns>
        public static bool LockTake(string lockKey=null,string lockToken=null)
        {
            if (string.IsNullOrWhiteSpace(lockKey))
            {
                lockKey = RedisLockKey;
            }

            if (string.IsNullOrWhiteSpace(lockToken))
            {
                lockToken = RedisLockToken;
            }
            return Db.LockTake(RedisLockKey, RedisLockToken, TimeSpan.FromSeconds(10));
        }
        /// <summary>
        ///  释放redis事务锁
        /// </summary>
        /// <param name="lockKey"></param>
        /// <param name="lockToken"></param>
        /// <returns></returns>
        public static bool LockRelease(string lockKey = null, string lockToken = null)
        {
            if (string.IsNullOrWhiteSpace(lockKey))
            {
                lockKey = RedisLockKey;
            }

            if (string.IsNullOrWhiteSpace(lockToken))
            {
                lockToken = RedisLockToken;
            }
            return Db.LockRelease(RedisLockKey, RedisLockToken);
        }

        public static string GetStringByKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "";
            }
           return Db.StringGet(key);
        }

        public static T GetDataByKey<T>(string key)
        {
            var strValue=GetStringByKey(key);
            if (string.IsNullOrWhiteSpace(strValue))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(strValue);
        }

        public static bool SetString(string key, string value,TimeSpan? expiry=null)
        {
            if (string.IsNullOrWhiteSpace(key)||value==null)
            {
                return false;
            }
           return  Db.StringSet(key,value,  expiry );
        }

        public static bool SetData<T>(string key, T value, TimeSpan? expiry = null)
        {
            var strValue = JsonConvert.SerializeObject(value);
            return SetString(key,strValue, expiry);
        }
    }
    #endregion
}
