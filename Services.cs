// Ziua 4 - planificarea modulelor aplicatiei
using MySql.Data.MySqlClient;

namespace CentruInstruire
{
    
    public static class CursantService
    {
        public static List<Cursant> GetAll(string cautare = "")
        {
            var lista = new List<Cursant>();
            using var conn = Database.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Cursant WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(cautare))
                sql += " AND (Nume LIKE @c OR Prenume LIKE @c OR Email LIKE @c)";
            sql += " ORDER BY Nume, Prenume";

            using var cmd = new MySqlCommand(sql, conn);
            if (!string.IsNullOrWhiteSpace(cautare))
                cmd.Parameters.AddWithValue("@c", $"%{cautare}%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Cursant
                {
                    IdCursant = reader.GetInt32("IdCursant"),
                    Nume = reader.GetString("Nume"),
                    Prenume = reader.GetString("Prenume"),
                    Telefon = reader.IsDBNull(reader.GetOrdinal("Telefon")) ? "" : reader.GetString("Telefon"),
                    Email = reader.GetString("Email")
                });
            }
            return lista;
        }

        public static void Adauga(Cursant c)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = "INSERT INTO Cursant (Nume, Prenume, Telefon, Email) VALUES (@n, @p, @t, @e)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", c.Nume);
            cmd.Parameters.AddWithValue("@p", c.Prenume);
            cmd.Parameters.AddWithValue("@t", c.Telefon);
            cmd.Parameters.AddWithValue("@e", c.Email);
            cmd.ExecuteNonQuery();
        }

        public static void Modifica(Cursant c)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = "UPDATE Cursant SET Nume=@n, Prenume=@p, Telefon=@t, Email=@e WHERE IdCursant=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", c.Nume);
            cmd.Parameters.AddWithValue("@p", c.Prenume);
            cmd.Parameters.AddWithValue("@t", c.Telefon);
            cmd.Parameters.AddWithValue("@e", c.Email);
            cmd.Parameters.AddWithValue("@id", c.IdCursant);
            cmd.ExecuteNonQuery();
        }

        public static void Sterge(int id)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = "DELETE FROM Cursant WHERE IdCursant=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static bool EmailExista(string email, int exceptIdCursant = 0)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = "SELECT COUNT(*) FROM Cursant WHERE Email=@e AND IdCursant<>@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@id", exceptIdCursant);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }

    
    public static class CursService
    {
        public static List<Curs> GetAll(string formator = "", int durata = 0)
        {
            var lista = new List<Curs>();
            using var conn = Database.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Curs WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(formator))
                sql += " AND Formator LIKE @f";
            if (durata > 0)
                sql += " AND DurataZile=@d";
            sql += " ORDER BY Denumire";

            using var cmd = new MySqlCommand(sql, conn);
            if (!string.IsNullOrWhiteSpace(formator))
                cmd.Parameters.AddWithValue("@f", $"%{formator}%");
            if (durata > 0)
                cmd.Parameters.AddWithValue("@d", durata);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Curs
                {
                    IdCurs = reader.GetInt32("IdCurs"),
                    Denumire = reader.GetString("Denumire"),
                    Formator = reader.GetString("Formator"),
                    Pret = reader.GetDecimal("Pret"),
                    DurataZile = reader.GetInt32("DurataZile")
                });
            }
            return lista;
        }

        public static void Adauga(Curs c)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = "INSERT INTO Curs (Denumire, Formator, Pret, DurataZile) VALUES (@d, @f, @p, @dz)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@d", c.Denumire);
            cmd.Parameters.AddWithValue("@f", c.Formator);
            cmd.Parameters.AddWithValue("@p", c.Pret);
            cmd.Parameters.AddWithValue("@dz", c.DurataZile);
            cmd.ExecuteNonQuery();
        }

        public static void Modifica(Curs c)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = "UPDATE Curs SET Denumire=@d, Formator=@f, Pret=@p, DurataZile=@dz WHERE IdCurs=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@d", c.Denumire);
            cmd.Parameters.AddWithValue("@f", c.Formator);
            cmd.Parameters.AddWithValue("@p", c.Pret);
            cmd.Parameters.AddWithValue("@dz", c.DurataZile);
            cmd.Parameters.AddWithValue("@id", c.IdCurs);
            cmd.ExecuteNonQuery();
        }

        public static void Sterge(int id)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = "DELETE FROM Curs WHERE IdCurs=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }

   
    public static class InscriereService
    {
        public static List<Inscriere> GetByCursant(int idCursant)
        {
            var lista = new List<Inscriere>();
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = @"SELECT i.*, CONCAT(c.Nume,' ',c.Prenume) AS NumeCursant,
                           cu.Denumire AS DenumireCurs, cu.Pret AS PretCurs
                           FROM Inscriere i
                           JOIN Cursant c ON i.IdCursant=c.IdCursant
                           JOIN Curs cu ON i.IdCurs=cu.IdCurs
                           WHERE i.IdCursant=@id
                           ORDER BY i.DataInscriere DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idCursant);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Inscriere
                {
                    IdInscriere = reader.GetInt32("IdInscriere"),
                    IdCursant = reader.GetInt32("IdCursant"),
                    IdCurs = reader.GetInt32("IdCurs"),
                    DataInscriere = reader.GetDateTime("DataInscriere"),
                    StatusPlata = reader.GetString("StatusPlata"),
                    NumeCursant = reader.GetString("NumeCursant"),
                    DenumireCurs = reader.GetString("DenumireCurs"),
                    PretCurs = reader.GetDecimal("PretCurs")
                });
            }
            return lista;
        }

        public static void Adauga(Inscriere i)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = "INSERT INTO Inscriere (IdCursant, IdCurs, DataInscriere, StatusPlata) VALUES (@ic, @icu, @d, @s)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ic", i.IdCursant);
            cmd.Parameters.AddWithValue("@icu", i.IdCurs);
            cmd.Parameters.AddWithValue("@d", i.DataInscriere.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@s", i.StatusPlata);
            cmd.ExecuteNonQuery();
        }

        public static void Sterge(int id)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = "DELETE FROM Inscriere WHERE IdInscriere=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static bool ExistaInscriere(int idCursant, int idCurs)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = "SELECT COUNT(*) FROM Inscriere WHERE IdCursant=@ic AND IdCurs=@icu";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ic", idCursant);
            cmd.Parameters.AddWithValue("@icu", idCurs);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static List<RaportCursant> GetRaport()
        {
            var lista = new List<RaportCursant>();
            using var conn = Database.GetConnection();
            conn.Open();
            string sql = @"SELECT CONCAT(c.Nume,' ',c.Prenume) AS NumeComplet,
                           COUNT(i.IdInscriere) AS NrInscrieri,
                           SUM(CASE WHEN i.StatusPlata='Achitat' THEN cu.Pret ELSE 0 END) AS SumaTotala
                           FROM Cursant c
                           LEFT JOIN Inscriere i ON c.IdCursant=i.IdCursant
                           LEFT JOIN Curs cu ON i.IdCurs=cu.IdCurs
                           GROUP BY c.IdCursant, c.Nume, c.Prenume
                           ORDER BY SumaTotala DESC";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new RaportCursant
                {
                    NumeComplet = reader.GetString("NumeComplet"),
                    NrInscrieri = reader.GetInt32("NrInscrieri"),
                    SumaTotala = reader.IsDBNull(reader.GetOrdinal("SumaTotala")) ? 0 : reader.GetDecimal("SumaTotala")
                });
            }
            return lista;
        }

        public static (int TotalCursanti, decimal SumaTotala, decimal MediePerCursant, string CursCeleMaiMulteInscrieri) GetStatistici()
        {
            using var conn = Database.GetConnection();
            conn.Open();

            int totalCursanti = Convert.ToInt32(new MySqlCommand("SELECT COUNT(*) FROM Cursant", conn).ExecuteScalar());
            decimal sumaTotala = Convert.ToDecimal(new MySqlCommand(
                "SELECT COALESCE(SUM(cu.Pret),0) FROM Inscriere i JOIN Curs cu ON i.IdCurs=cu.IdCurs WHERE i.StatusPlata='Achitat'", conn).ExecuteScalar());
            decimal medie = totalCursanti > 0 ? sumaTotala / totalCursanti : 0;

            string cursFruntas = "";
            string sqlCurs = @"SELECT cu.Denumire, COUNT(*) AS NrInscrieri 
                               FROM Inscriere i JOIN Curs cu ON i.IdCurs=cu.IdCurs 
                               GROUP BY cu.IdCurs, cu.Denumire 
                               ORDER BY NrInscrieri DESC LIMIT 1";
            using var cmd = new MySqlCommand(sqlCurs, conn);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                cursFruntas = $"{reader.GetString("Denumire")} ({reader.GetInt32("NrInscrieri")} inscrieri)";

            return (totalCursanti, sumaTotala, medie, cursFruntas);
        }
    }
}