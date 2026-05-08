// Ziua 2 - analiza arhitecturii si fluxului de date
using MySql.Data.MySqlClient;
// Ziua 5 - analiza structurii bazei de date
// Ziua 9 - proiectarea bazei de date pentru cursuri

// Ziua 10 - crearea si configurarea bazei de date
// Ziua 11 - administrarea obiectelor bazei de date
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