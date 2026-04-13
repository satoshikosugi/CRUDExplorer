using System.Data.SqlClient;

public class UserRepository
{
    private readonly string _connectionString;

    public User GetUser(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT id, name, email FROM users WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapUser(reader) : null;
    }

    public void CreateUser(string name, string email)
    {
        using var conn = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("", conn);
        cmd.CommandText = "INSERT INTO users (name, email) VALUES (@name, @email)";
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@email", email);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void UpdateUser(int id, string email)
    {
        using var conn = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("", conn);
        cmd.CommandText = "UPDATE users SET email = @email WHERE id = @id";
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void DeleteUser(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("", conn);
        cmd.CommandText = "DELETE FROM users WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();
        cmd.ExecuteNonQuery();
    }
}
