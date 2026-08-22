using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

public static class DatabaseHelper
{
    private static readonly string connStr =
        ConfigurationManager.ConnectionStrings["LoginDBConnection"].ConnectionString;

    public static bool TestConnection()
    {
        try
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Could not connect to the database.\n\n" + ex.Message,
                             "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(connStr);
    }

    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    public static bool UsernameExists(string username)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            string query = "SELECT COUNT(*) FROM dbo.Users WHERE Username = @username";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@username", username);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }

    public static bool RegisterUser(string username, string password, string email, string fullName)
    {
        string passwordHash = HashPassword(password);

        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            string query = @"INSERT INTO dbo.Users (Username, PasswordHash, Email, FullName)
                              VALUES (@username, @passwordHash, @email, @fullName)";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@fullName", fullName);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
    public static string ValidateLogin(string username, string password)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            string query = "SELECT PasswordHash, FullName FROM dbo.Users WHERE Username = @username";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@username", username);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedHash = reader["PasswordHash"].ToString();
                        string fullName = reader["FullName"] == DBNull.Value
                                           ? "" : reader["FullName"].ToString();

                        string inputHash = HashPassword(password);
                        if (inputHash == storedHash)
                            return fullName;
                    }
                }
            }
        }
        return null;
    }
    public static System.Data.DataTable GetUsers(string searchTerm = null)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string query = "SELECT UserID, Username, Email, CreatedAt FROM dbo.Users";

            if (!string.IsNullOrEmpty(searchTerm))
                query += " WHERE Username LIKE @term";

            using (SqlDataAdapter adapter = new SqlDataAdapter(query, con))
            {
                if (!string.IsNullOrEmpty(searchTerm))
                    adapter.SelectCommand.Parameters.AddWithValue("@term", "%" + searchTerm + "%");

                System.Data.DataTable dt = new System.Data.DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }
}