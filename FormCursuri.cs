// Ziua 12 - proiectarea interfetei pentru cursuri
// Ziua 13 - implementarea logicii de prelucrare pentru cursuri
// Ziua 24 - adaugarea filtrelor avansate in modulul Cursuri
namespace CentruInstruire
{
    public class FormCursuri : Form
    {
        private DataGridView grid = new DataGridView();
        private TextBox txtFiltruFormator = new TextBox();
        private TextBox txtFiltruDurata = new TextBox();
        private TextBox txtDenumire = new TextBox();
        private TextBox txtFormator = new TextBox();
        private TextBox txtPret = new TextBox();
        private TextBox txtDurata = new TextBox();
        private Button btnAdauga = new Button();
        private Button btnModifica = new Button();
        private Button btnSterge = new Button();
        private Button btnFiltreaza = new Button();
        private Button btnRefresh = new Button();
        private int selectedId = 0;

        public FormCursuri()
        {
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            this.Text = "Gestionare Cursuri";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            var panelLista = new Panel { Left = 10, Top = 10, Width = 580, Height = 540 };

            var lblFormator = new Label { Text = "Formator:", Top = 5, Left = 0, Width = 65 };
            txtFiltruFormator.Left = 68; txtFiltruFormator.Top = 2; txtFiltruFormator.Width = 140;

            var lblDurata = new Label { Text = "Zile:", Top = 5, Left = 215, Width = 35 };
            txtFiltruDurata.Left = 250; txtFiltruDurata.Top = 2; txtFiltruDurata.Width = 60;

            btnFiltreaza.Text = "Filtreaza"; btnFiltreaza.Left = 315; btnFiltreaza.Top = 0; btnFiltreaza.Width = 85;
            btnRefresh.Text = "Toti"; btnRefresh.Left = 405; btnRefresh.Top = 0; btnRefresh.Width = 60;

            grid.Left = 0; grid.Top = 35; grid.Width = 570; grid.Height = 495;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AllowUserToAddRows = false;
            grid.RowHeadersVisible = false;

            panelLista.Controls.AddRange(new Control[] { lblFormator, txtFiltruFormator, lblDurata, txtFiltruDurata, btnFiltreaza, btnRefresh, grid });

            var panelForm = new Panel { Left = 600, Top = 10, Width = 320, Height = 540 };

            int y = 10;
            panelForm.Controls.Add(MakeLabel("Denumire *:", 0, y));
            txtDenumire.Left = 110; txtDenumire.Top = y; txtDenumire.Width = 200; panelForm.Controls.Add(txtDenumire);

            y += 40;
            panelForm.Controls.Add(MakeLabel("Formator *:", 0, y));
            txtFormator.Left = 110; txtFormator.Top = y; txtFormator.Width = 200; panelForm.Controls.Add(txtFormator);

            y += 40;
            panelForm.Controls.Add(MakeLabel("Pret (lei) *:", 0, y));
            txtPret.Left = 110; txtPret.Top = y; txtPret.Width = 200; panelForm.Controls.Add(txtPret);

            y += 40;
            panelForm.Controls.Add(MakeLabel("Durata (zile)*:", 0, y));
            txtDurata.Left = 110; txtDurata.Top = y; txtDurata.Width = 200; panelForm.Controls.Add(txtDurata);

            y += 60;
            btnAdauga.Text = "Adauga"; btnAdauga.Left = 0; btnAdauga.Top = y; btnAdauga.Width = 95;
            btnAdauga.BackColor = Color.SteelBlue; btnAdauga.ForeColor = Color.White;
            panelForm.Controls.Add(btnAdauga);

            btnModifica.Text = "Modifica"; btnModifica.Left = 100; btnModifica.Top = y; btnModifica.Width = 95;
            btnModifica.BackColor = Color.DarkOrange; btnModifica.ForeColor = Color.White;
            panelForm.Controls.Add(btnModifica);

            btnSterge.Text = "Sterge"; btnSterge.Left = 200; btnSterge.Top = y; btnSterge.Width = 95;
            btnSterge.BackColor = Color.Crimson; btnSterge.ForeColor = Color.White;
            panelForm.Controls.Add(btnSterge);

            this.Controls.AddRange(new Control[] { panelLista, panelForm });

            grid.SelectionChanged += Grid_SelectionChanged;
            btnAdauga.Click += BtnAdauga_Click;
            btnModifica.Click += BtnModifica_Click;
            btnSterge.Click += BtnSterge_Click;
            btnFiltreaza.Click += (s, e) =>
            {
                int d = int.TryParse(txtFiltruDurata.Text, out int dv) ? dv : 0;
                LoadData(txtFiltruFormator.Text, d);
            };
            btnRefresh.Click += (s, e) => { txtFiltruFormator.Clear(); txtFiltruDurata.Clear(); LoadData(); };
        }

        private Label MakeLabel(string text, int x, int y) =>
            new Label { Text = text, Left = x, Top = y + 3, Width = 105 };

        private void LoadData(string formator = "", int durata = 0)
        {
            try
            {
                grid.SelectionChanged -= Grid_SelectionChanged;

                var lista = CursService.GetAll(formator, durata);
                grid.DataSource = lista.Select(c => new
                {
                    c.IdCurs,
                    c.Denumire,
                    c.Formator,
                    Pret = c.Pret + " lei",
                    c.DurataZile
                }).ToList();
                if (grid.Columns.Contains("IdCurs")) { grid.Columns["IdCurs"].HeaderText = "ID"; grid.Columns["IdCurs"].Width = 40; }
                if (grid.Columns.Contains("DurataZile")) grid.Columns["DurataZile"].HeaderText = "Zile";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                grid.SelectionChanged += Grid_SelectionChanged;
            }
        }

        private void Grid_SelectionChanged(object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            var row = grid.SelectedRows[0];
            if (row.Cells["IdCurs"]?.Value == null) return;
            try
            {
                selectedId = (int)row.Cells["IdCurs"].Value;
                txtDenumire.Text = row.Cells["Denumire"].Value?.ToString() ?? "";
                txtFormator.Text = row.Cells["Formator"].Value?.ToString() ?? "";
                var pretStr = row.Cells["Pret"].Value?.ToString()?.Replace(" lei", "") ?? "";
                txtPret.Text = pretStr;
                txtDurata.Text = row.Cells["DurataZile"].Value?.ToString() ?? "";
            }
            catch { }
        }

        private bool Valideaza()
        {
            if (string.IsNullOrWhiteSpace(txtDenumire.Text) ||
                string.IsNullOrWhiteSpace(txtFormator.Text) ||
                string.IsNullOrWhiteSpace(txtPret.Text) ||
                string.IsNullOrWhiteSpace(txtDurata.Text))
            {
                MessageBox.Show("Toate campurile marcate cu * sunt obligatorii!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPret.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal pret) || pret <= 0)
            {
                MessageBox.Show("Pretul trebuie sa fie un numar pozitiv mai mare decat 0!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtDurata.Text, out int d) || d <= 0)
            {
                MessageBox.Show("Durata trebuie sa fie un numar intreg pozitiv!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private Curs GetFromForm()
        {
            decimal.TryParse(txtPret.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal pret);
            int.TryParse(txtDurata.Text, out int durata);
            return new Curs
            {
                IdCurs = selectedId,
                Denumire = txtDenumire.Text.Trim(),
                Formator = txtFormator.Text.Trim(),
                Pret = pret,
                DurataZile = durata
            };
        }

        private void BtnAdauga_Click(object? sender, EventArgs e)
        {
            if (!Valideaza()) return;
            try
            {
                CursService.Adauga(GetFromForm());
                LoadData();
                ClearForm();
                MessageBox.Show("Curs adaugat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnModifica_Click(object? sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Selectati un curs!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!Valideaza()) return;
            try
            {
                CursService.Modifica(GetFromForm());
                LoadData();
                MessageBox.Show("Curs modificat!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnSterge_Click(object? sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Selectati un curs!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var rez = MessageBox.Show("Stergeti cursul si toate inscrierile aferente?", "Confirmare",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rez == DialogResult.Yes)
            {
                try
                {
                    CursService.Sterge(selectedId);
                    ClearForm();
                    LoadData();
                    MessageBox.Show("Curs sters!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void ClearForm()
        {
            selectedId = 0;
            txtDenumire.Clear(); txtFormator.Clear(); txtPret.Clear(); txtDurata.Clear();
        }
    }
}