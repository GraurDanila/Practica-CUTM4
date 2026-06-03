// Ziua 37 - adaugarea comentariilor si documentatiei complete
namespace CentruInstruire
{
    public class Cursant
    {
        public int IdCursant { get; set; }
        public string Nume { get; set; } = "";
        public string Prenume { get; set; } = "";
        public string Telefon { get; set; } = "";
        public string Email { get; set; } = "";

        public string NumeComplet => $"{Nume} {Prenume}";

        public override string ToString() => NumeComplet;
    }

    public class Curs
    {
        public int IdCurs { get; set; }
        public string Denumire { get; set; } = "";
        public string Formator { get; set; } = "";
        public decimal Pret { get; set; }
        public int DurataZile { get; set; }

        public override string ToString() => Denumire;
    }

    public class Inscriere
    {
        public int IdInscriere { get; set; }
        public int IdCursant { get; set; }
        public int IdCurs { get; set; }
        public DateTime DataInscriere { get; set; }
        public string StatusPlata { get; set; } = "Neachitat";

        // Pentru afisare
        public string NumeCursant { get; set; } = "";
        public string DenumireCurs { get; set; } = "";
        public decimal PretCurs { get; set; }
    }

    public class RaportCursant
    {
        public string NumeComplet { get; set; } = "";
        public int NrInscrieri { get; set; }
        public decimal SumaTotala { get; set; }
    }
}