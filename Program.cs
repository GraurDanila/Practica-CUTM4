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