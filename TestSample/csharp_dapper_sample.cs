using Dapper;
using System.Data;

public class OrderRepository
{
    private readonly IDbConnection _conn;

    public async Task<User> GetUserAsync(int id)
    {
        return await _conn.QueryAsync<User>(
            "SELECT * FROM users WHERE id = @Id",
            new { Id = id });
    }

    public async Task CreateOrderAsync(int userId, decimal total)
    {
        await _conn.ExecuteAsync(
            "INSERT INTO orders (user_id, total) VALUES (@UserId, @Total)",
            new { UserId = userId, Total = total });
    }

    public async Task UpdateOrderStatusAsync(int id, string status)
    {
        await _conn.ExecuteAsync(
            "UPDATE orders SET status = @Status WHERE id = @Id",
            new { Status = status, Id = id });
    }

    public async Task DeleteOrderItemsAsync(int orderId)
    {
        await _conn.ExecuteAsync(
            "DELETE FROM order_items WHERE order_id = @OrderId",
            new { OrderId = orderId });
    }
}
