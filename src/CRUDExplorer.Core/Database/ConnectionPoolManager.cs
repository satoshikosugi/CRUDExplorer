using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;

namespace CRUDExplorer.Core.Database;

/// <summary>
/// データベース接続プール管理
/// </summary>
public class ConnectionPoolManager : IDisposable
{
    private readonly ConcurrentDictionary<string, ConnectionPool> _pools = new(StringComparer.OrdinalIgnoreCase);
    private bool _disposed;

    /// <summary>
    /// プールから接続を取得（自動でプール作成）
    /// </summary>
    public async Task<IDbConnection> GetConnectionAsync(string connectionName, DatabaseConnection connectionInfo)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ConnectionPoolManager));
        }

        var pool = _pools.GetOrAdd(connectionName, _ => new ConnectionPool(connectionInfo));
        return await pool.GetConnectionAsync();
    }

    /// <summary>
    /// 接続をプールに返却
    /// </summary>
    public void ReturnConnection(string connectionName, IDbConnection connection)
    {
        if (_disposed || !_pools.TryGetValue(connectionName, out var pool))
        {
            connection?.Dispose();
            return;
        }

        pool.ReturnConnection(connection);
    }

    /// <summary>
    /// プールをクリア
    /// </summary>
    public void ClearPool(string connectionName)
    {
        if (_pools.TryRemove(connectionName, out var pool))
        {
            pool.Dispose();
        }
    }

    /// <summary>
    /// 全プールをクリア
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in _pools.Values)
        {
            pool.Dispose();
        }
        _pools.Clear();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            ClearAllPools();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// 個別接続プール
/// </summary>
internal class ConnectionPool : IDisposable
{
    private readonly DatabaseConnection _connectionInfo;
    private readonly ConcurrentBag<IDbConnection> _availableConnections = new();
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxPoolSize;
    private int _currentSize;
    private bool _disposed;

    public ConnectionPool(DatabaseConnection connectionInfo, int maxPoolSize = 10)
    {
        _connectionInfo = connectionInfo;
        _maxPoolSize = maxPoolSize;
        _semaphore = new SemaphoreSlim(maxPoolSize, maxPoolSize);
    }

    public async Task<IDbConnection> GetConnectionAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ConnectionPool));
        }

        await _semaphore.WaitAsync();

        if (_availableConnections.TryTake(out var connection))
        {
            // Verify connection is still valid
            if (connection.State == ConnectionState.Open)
            {
                return connection;
            }
            connection.Dispose();
            Interlocked.Decrement(ref _currentSize);
        }

        // Create new connection
        var factory = DbProviderFactory.GetConnectionFactory(_connectionInfo.DatabaseType);
        var newConnection = factory.CreateConnection(_connectionInfo.ConnectionString);

        try
        {
            if (newConnection is DbConnection dbConnection)
            {
                await dbConnection.OpenAsync();
            }
            else
            {
                newConnection.Open();
            }
            Interlocked.Increment(ref _currentSize);
            return newConnection;
        }
        catch
        {
            _semaphore.Release();
            newConnection.Dispose();
            throw;
        }
    }

    public void ReturnConnection(IDbConnection connection)
    {
        if (_disposed)
        {
            connection?.Dispose();
            return;
        }

        if (connection != null && connection.State == ConnectionState.Open)
        {
            _availableConnections.Add(connection);
        }
        else
        {
            connection?.Dispose();
            Interlocked.Decrement(ref _currentSize);
        }

        _semaphore.Release();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _semaphore.Dispose();

            while (_availableConnections.TryTake(out var connection))
            {
                connection?.Dispose();
            }
        }
    }
}
