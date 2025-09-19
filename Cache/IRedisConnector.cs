using StackExchange.Redis;
namespace MesEnterprise.Cache
{
   
        public interface IRedisConnector { ConnectionMultiplexer GetConnection(); }
        public class RedisConnector : IRedisConnector
        {
            private readonly Lazy<ConnectionMultiplexer> _conn;
            public RedisConnector(IConfiguration cfg) { _conn = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(cfg.GetConnectionString("Redis") ?? "localhost:6379")); }
            public ConnectionMultiplexer GetConnection() => _conn.Value;
        }
    
}
