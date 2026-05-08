// Ziua 15 - proiectarea interfetei pentru inscrieri
namespace CentruInstruire
{
    public class FormInscrieri : Form
    {
        private ComboBox cbCursant = new ComboBox();
        private DataGridView gridInscrieri = new DataGridView();
        private ComboBox cbCurs = new ComboBox();
        private DateTimePicker dtpData = new DateTimePicker();
        private ComboBox cbStatus = new ComboBox();
        private Button btnInscriere = new Button();
        private Button btnAnulare = new Button();
        private int selectedInscriereId = 0;

        public FormInscrieri()
        {
            InitUI();
            LoadCursanti();
            LoadCursuri();
        }

        private void InitUI()
        {
            this.Text = "Gestionare Inscrieri";
            this.Size = new Size(850, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ---- Selectie cursant ----
            var lblCursant = new Label { Text = "Selecteaza cursantul:", Left = 10, Top = 15, Width = 150 };
            cbCursant.Left = 165; cbCursant.Top = 12; cbCursant.Width = 250;
            cbCursant.DropDownStyle = ComboBoxStyle.DropDownList;

            var btnLoad = new Button { Text = "Afiseaza inscrierile", Left = 420, Top = 10, Width = 150 };
            btnLoad.Click += (s, e) => LoadInscrieri();

            // ---- Grid inscrieri ----
            var lblInscrieri = new Label { Text = "Inscrierile cursantului selectat:", Left = 10, Top = 50, Width = 220 };

            gridInscrieri.Left = 10; gridInscrieri.Top = 70; gridInscrieri.Width = 820; gridInscrieri.Height = 300;
            gridInscrieri.ReadOnly = true;
            gridInscrieri.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridInscrieri.MultiSelect = false;
            gridInscrieri.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridInscrieri.AllowUserToAddRows = false;
            gridInscrieri.RowHeadersVisible = false;
            gridInscrieri.SelectionChanged += Grid_SelectionChanged;

            // ---- Formular inscriere noua ----
            var lblNou = new Label { Text = "Inscriere noua:", Left = 10, Top = 385, Width = 120, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

            var lblCurs = new Label { Text = "Curs *:", Left = 10, Top = 415, Width = 60 };
            cbCurs.Left = 75; cbCurs.Top = 412; cbCurs.Width = 250; cbCurs.DropDownStyle = ComboBoxStyle.DropDownList;

            var lblData = new Label { Text = "Data *:", Left = 340, Top = 415, Width = 50 };
            dtpData.Left = 395; dtpData.Top = 412; dtpData.Width = 150; dtpData.Format = DateTimePickerFormat.Short;

            var lblStatus = new Label { Text = "Status *:", Left = 555, Top = 415, Width = 60 };
            cbStatus.Left = 620; cbStatus.Top = 412; cbStatus.Width = 130; cbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cbStatus.Items.AddRange(new object[] { "Neachitat", "Achitat" });
            cbStatus.SelectedIndex = 0;

            btnInscriere.Text = "Adauga Inscriere";
            btnInscriere.Left = 10; btnInscriere.Top = 455; btnInscriere.Width = 150;
            btnInscriere.BackColor = Color.SteelBlue; btnInscriere.ForeColor = Color.White;
            btnInscriere.Click += BtnInscriere_Click;

            btnAnulare.Text = "Anuleaza Inscriere Selectata";
            btnAnulare.Left = 170; btnAnulare.Top = 455; btnAnulare.Width = 210;
            btnAnulare.BackColor = Color.Crimson; btnAnulare.ForeColor = Color.White;
            btnAnulare.Click += BtnAnulare_Click;

            this.Controls.AddRange(new Control[]
            {
                lblCursant, cbCursant, btnLoad,
                lblInscrieri, gridInscrieri,
                lblNou, lblCurs, cbCurs, lblData, dtpData, lblStatus, cbStatus,
                btnInscriere, btnAnulare
            });
        }

        private void LoadCursanti()
        {
            try
            {
                cbCursant.DataSource = CursantService.GetAll();
                cbCursant.DisplayMember = "NumeComplet";
                cbCursant.ValueMember = "IdCursant";
            }
            catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message); }
        }

        private void LoadCursuri()
        {
            try
            {
                cbCurs.DataSource = CursService.GetAll();
                cbCurs.DisplayMember = "Denumire";
                cbCurs.ValueMember = "IdCurs";
            }
            catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message); }
        }

        private void LoadInscrieri()
        {
            if (cbCursant.SelectedValue == null) return;
            int idCursant = (int)cbCursant.SelectedValue;
            try
            {
                var lista = InscriereService.GetByCursant(idCursant);
                gridInscrieri.DataSource = lista.Select(i => new
                {
                    i.IdInscriere,
                    Curs = i.DenumireCurs,
                    Data = i.DataInscriere.ToString("dd.MM.yyyy"),
                    Status = i.StatusPlata,
                    Pret = i.PretCurs + " lei"
                }).ToList();
                if (gridInscrieri.Columns.Contains("IdInscriere"))
                    gridInscrieri.Columns["IdInscriere"].Width = 40;
            }
            catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message); }
        }

        private void Grid_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridInscrieri.SelectedRows.Count > 0)
                selectedInscriereId = (int)gridInscrieri.SelectedRows[0].Cells["IdInscriere"].Value;
        }

        private void BtnInscriere_Click(object? sender, EventArgs e)
        {
            if (cbCursant.SelectedValue == null || cbCurs.SelectedValue == null)
            {
                MessageBox.Show("Selectati cursantul si cursul!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idCursant = (int)cbCursant.SelectedValue;
            int idCurs = (int)cbCurs.SelectedValue;

            if (InscriereService.ExistaInscriere(idCursant, idCurs))
            {
                MessageBox.Show("Cursantul este deja inscris la acest curs!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                InscriereService.Adauga(new Inscriere
                {
                    IdCursant = idCursant,
                    IdCurs = idCurs,
                    DataInscriere = dtpData.Value,
                    StatusPlata = cbStatus.SelectedItem?.ToString() ?? "Neachitat"
                });
                LoadInscrieri();
                MessageBox.Show("Inscriere adaugata cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnAnulare_Click(object? sender, EventArgs e)
        {
            if (selectedInscriereId == 0)
            {
                MessageBox.Show("Selectati o inscriere din lista!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var rez = MessageBox.Show("Anulati inscrierea selectata?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rez == DialogResult.Yes)
            {
                try
                {
                    InscriereService.Sterge(selectedInscriereId);
                    selectedInscriereId = 0;
                    LoadInscrieri();
                    MessageBox.Show("Inscriere anulata!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
    }
}