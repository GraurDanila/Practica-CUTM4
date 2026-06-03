// Ziua 20 - raport intermediar luna 1
// Ziua 39 - elaborarea raportului final al stagiului de practica
// Ziua 40 - versiunea finala - stagiu de practica incheiat
using System.Windows.Forms;
using WinFormsApp1;

namespace CentruInstruire
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}