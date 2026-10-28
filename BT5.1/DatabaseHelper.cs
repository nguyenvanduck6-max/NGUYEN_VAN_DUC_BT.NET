using System;
using System.Security.Cryptography;
using System.Text;
using Npgsql;

namespace BaiTap1
{
    public class DatabaseHelper
    {
        private static string connectionString =
            "Host=localhost;Port=5432;Database=login_db;Username=postgres;Password=123456";

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder result = new StringBuilder();

                foreach (byte b in hash)
                {
                    result.Append(b.ToString("x2"));
                }

                return result.ToString();
            }
        }

        public static bool CheckLogin(string username, string password)
        {
            using (NpgsqlConnection conn =
                new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT COUNT(*)
                    FROM users
                    WHERE username = @username
                    AND password_hash = @passwordHash";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue(
                        "@passwordHash", HashPassword(password));

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
        }
    }
}
