// Ziua 14 - implementarea extractiei datelor si rapoartelor
// Ziua 26 - implementarea raportului pentru cursanti activi
// Ziua 27 - implementarea rapoartelor statistice pentru cursuri
using System.Text;

namespace CentruInstruire
{
    public class FormRaport : Form
    {
        private DataGridView gridRaport = new DataGridView();
        private Label lblTotalCursanti = new Label();
        private Label lblSumaTotala = new Label();
        private Label lblMedie = new Label();
        private Label lblCursFruntas = new Label();
        private Button btnExportTxt = new Button();

        public FormRaport()
        {
            InitUI();
            LoadRaport();
        }

        private void InitUI()
        {
            this.Text = "Raport Sumar - Participare si Plati";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            var lblTitlu = new Label
            {
                Text = "RAPORT SUMAR - PARTICIPARE SI PLATI",
                Left = 10,
                Top = 10,
                Width = 660,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Grid raport
            gridRaport.Left = 10; gridRaport.Top = 45; gridRaport.Width = 660; gridRaport.Height = 280;
            gridRaport.ReadOnly = true;
            gridRaport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridRaport.AllowUserToAddRows = false;
            gridRaport.RowHeadersVisible = false;
            gridRaport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Statistici
            var lblStatTitlu = new Label
            {
                Text = "Statistici generale:",
                Left = 10,
                Top = 340,
                Width = 200,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            lblTotalCursanti.Left = 10; lblTotalCursanti.Top = 365; lblTotalCursanti.Width = 320;
            lblSumaTotala.Left = 10; lblSumaTotala.Top = 388; lblSumaTotala.Width = 320;
            lblMedie.Left = 10; lblMedie.Top = 411; lblMedie.Width = 320;
            lblCursFruntas.Left = 10; lblCursFruntas.Top = 434; lblCursFruntas.Width = 660;

            btnExportTxt.Text = "Export TXT";
            btnExportTxt.Left = 10; btnExportTxt.Top = 470; btnExportTxt.Width = 130;
            btnExportTxt.BackColor = Color.ForestGreen; btnExportTxt.ForeColor = Color.White;
            btnExportTxt.Click += BtnExportTxt_Click;

            var btnInchide = new Button { Text = "Inchide", Left = 150, Top = 470, Width = 100 };
            btnInchide.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblTitlu, gridRaport,
                lblStatTitlu, lblTotalCursanti, lblSumaTotala, lblMedie, lblCursFruntas,
                btnExportTxt, btnInchide
            });
        }

        private void LoadRaport()
        {
            try
            {
                // Raport principal
                var lista = InscriereService.GetRaport();
                gridRaport.DataSource = lista.Select(r => new
                {
                    NumeComplet = r.NumeComplet,
                    NrInscrieri = r.NrInscrieri,
                    SumaTotala = r.SumaTotala.ToString("N2") + " lei"
                }).ToList();

                if (gridRaport.Columns.Contains("NumeComplet"))
                    gridRaport.Columns["NumeComplet"].HeaderText = "Cursant";
                if (gridRaport.Columns.Contains("NrInscrieri"))
                    gridRaport.Columns["NrInscrieri"].HeaderText = "Nr. Inscrieri";
                if (gridRaport.Columns.Contains("SumaTotala"))
                    gridRaport.Columns["SumaTotala"].HeaderText = "Suma Achitata";

                // Statistici
                var (total, suma, medie, curs) = InscriereService.GetStatistici();
                lblTotalCursanti.Text = $"Total cursanti inscrisi: {total}";
                lblSumaTotala.Text = $"Suma totala incasata: {suma:N2} lei";
                lblMedie.Text = $"Media platilor per cursant: {medie:N2} lei";
                lblCursFruntas.Text = $"Cursul cu cele mai multe inscrieri: {curs}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la generarea raportului: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportTxt_Click(object? sender, EventArgs e)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("================================================");
                sb.AppendLine("   RAPORT SUMAR - CENTRU DE INSTRUIRE");
                sb.AppendLine($"   Data: {DateTime.Now:dd.MM.yyyy HH:mm}");
                sb.AppendLine("================================================");
                sb.AppendLine();
                sb.AppendLine($"{"Cursant",-30} {"Nr.Inscrieri",12} {"Suma Achitata",16}");
                sb.AppendLine(new string('-', 60));

                var lista = InscriereService.GetRaport();
                foreach (var r in lista)
                    sb.AppendLine($"{r.NumeComplet,-30} {r.NrInscrieri,12} {r.SumaTotala,14:N2} lei");

                sb.AppendLine(new string('-', 60));
                var (total, suma, medie, curs) = InscriereService.GetStatistici();
                sb.AppendLine();
                sb.AppendLine($"Total cursanti: {total}");
                sb.AppendLine($"Suma totala incasata: {suma:N2} lei");
                sb.AppendLine($"Media per cursant: {medie:N2} lei");
                sb.AppendLine($"Cursul fruntase: {curs}");

                using var dlg = new SaveFileDialog();
                dlg.Filter = "Fisiere text (*.txt)|*.txt";
                dlg.FileName = $"Raport_{DateTime.Now:yyyyMMdd_HHmm}.txt";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Raport exportat cu succes!\n" + dlg.FileName, "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la export: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}