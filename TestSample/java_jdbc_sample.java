import java.sql.*;

public class UserRepository {
    private Connection conn;

    public User findUser(int id) throws SQLException {
        PreparedStatement ps = conn.prepareStatement(
            "SELECT id, name, email FROM users WHERE id = ?");
        ps.setInt(1, id);
        ResultSet rs = ps.executeQuery();
        return mapUser(rs);
    }

    public void createUser(String name, String email) throws SQLException {
        PreparedStatement ps = conn.prepareStatement(
            "INSERT INTO users (name, email) VALUES (?, ?)");
        ps.setString(1, name);
        ps.setString(2, email);
        ps.executeUpdate();
    }

    public void updateUser(int id, String email) throws SQLException {
        PreparedStatement ps = conn.prepareStatement(
            "UPDATE users SET email = ? WHERE id = ?");
        ps.setString(1, email);
        ps.setInt(2, id);
        ps.executeUpdate();
    }

    public void deleteUser(int id) throws SQLException {
        PreparedStatement ps = conn.prepareStatement(
            "DELETE FROM users WHERE id = ?");
        ps.setInt(1, id);
        ps.executeUpdate();
    }

    public List<Order> findOrdersByUser(int userId) throws SQLException {
        PreparedStatement ps = conn.prepareStatement(
            "SELECT u.id, u.name, o.order_id, o.total FROM users u JOIN orders o ON u.id = o.user_id WHERE u.id = ?");
        ps.setInt(1, userId);
        ResultSet rs = ps.executeQuery();
        return mapOrders(rs);
    }
}
