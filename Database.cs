using MySql.Data.MySqlClient;

namespace CentruInstruire
{
    public static class Database
    {

        private static string connectionString =
            "Server=localhost;Database=CentruInstruire;Uid=root;Pwd=mysql;CharSet=utf8mb4;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static void SetPassword(string password)
        {
            connectionString =
                $"Server=localhost;Database=CentruInstruire;Uid=root;Pwd={password};CharSet=utf8mb4;";
        }

        public static bool TestConnection()
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}