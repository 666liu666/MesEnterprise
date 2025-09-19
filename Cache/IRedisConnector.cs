using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MesEnterprise.Cache;

public interface IRedisConnector
{
    ConnectionMultiplexer GetConnection();
}

public sealed class RedisConnector : IRedisConnector, IDisposable
{
    private readonly Lazy<ConnectionMultiplexer> _connection;

    public RedisConnector(IConfiguration configuration)
    {
        _connection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost:6379"));
    }

    public ConnectionMultiplexer GetConnection() => _connection.Value;

    public void Dispose()
    {
        if (_connection.IsValueCreated)
        {
            _connection.Value.Dispose();
        }
    }
}
