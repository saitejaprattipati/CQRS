using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Extensions.Configuration;
using System.IO;


namespace Author.Core.Services.Rediscache
{
    public class RedisConnect
    {
        private static RedisCacheConnectionSettings RedisConnection;
        public RedisConnect()
        {
            RedisConnection = ReadConfiguration();
        }
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(RedisConnection.RedisCacheName + ",abortConnect=false,ssl=true,password=" + RedisConnection.RedisCachePassword);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }



        private RedisCacheConnectionSettings ReadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            var config = builder.Build();
            var connectionInfo = new RedisCacheConnectionSettings
            {
                RedisCacheName = config["RedisCacheName"],
                RedisCachePassword = config["RedisCachePassword"]
            };

            if (string.IsNullOrEmpty(connectionInfo.RedisCacheName))
            {
                builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json");
                config = builder.Build();
                connectionInfo = new RedisCacheConnectionSettings
                {
                    RedisCacheName = config["RedisCacheName"],
                    RedisCachePassword = config["RedisCachePassword"]
                };
            }

            return connectionInfo;
        }
    }
}
