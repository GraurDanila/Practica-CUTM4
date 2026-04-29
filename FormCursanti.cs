// Ziua 7 - proiectarea interfetei pentru cursanti
using System.Text.RegularExpressions;

namespace CentruInstruire
{
    public class FormCursanti : Form
    {
        private DataGridView grid = new DataGridView();
        private TextBox txtCautare = new TextBox();
        private TextBox txtNume = new TextBox();
        private TextBox txtPrenume = new TextBox();
        private TextBox txtTelefon = new TextBox();
        private TextBox txtEmail = new TextBox();
        private Button btnAdauga = new Button();
        private Button btnModifica = new Button();
        private Button btnSterge = new Button();
        private Button btnCauta = new Button();
        private Button btnRefresh = new Button();
        private int selectedId = 0;

        public FormCursanti()
        {
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            this.Text = "Gestionare Cursanti";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ---- Panel stanga: lista ----
            var panelLista = new Panel { Left = 10, Top = 10, Width = 550, Height = 540 };

            var lblCautare = new Label { Text = "Cauta (Nume/Email):", Top = 5, Left = 0, Width = 140 };
            txtCautare.Left = 145; txtCautare.Top = 2; txtCautare.Width = 250;
            btnCauta.Text = "Cauta"; btnCauta.Left = 400; btnCauta.Top = 0; btnCauta.Width = 70;
            btnRefresh.Text = "Toti"; btnRefresh.Left = 475; btnRefresh.Top = 0; btnRefresh.Width = 60;

            grid.Left = 0; grid.Top = 35; grid.Width = 540; grid.Height = 495;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AllowUserToAddRows = false;
            grid.RowHeadersVisible = false;

            panelLista.Controls.AddRange(new Control[] { lblCautare, txtCautare, btnCauta, btnRefresh, grid });

            // ---- Panel dreapta: formular ----
            var panelForm = new Panel { Left = 570, Top = 10, Width = 300, Height = 540 };

            int y = 10;
            panelForm.Controls.Add(MakeLabel("Nume *:", 0, y));
            txtNume.Left = 100; txtNume.Top = y; txtNume.Width = 190; panelForm.Controls.Add(txtNume);

            y += 40;
            panelForm.Controls.Add(MakeLabel("Prenume *:", 0, y));
            txtPrenume.Left = 100; txtPrenume.Top = y; txtPrenume.Width = 190; panelForm.Controls.Add(txtPrenume);

            y += 40;
            panelForm.Controls.Add(MakeLabel("Telefon:", 0, y));
            txtTelefon.Left = 100; txtTelefon.Top = y; txtTelefon.Width = 190; panelForm.Controls.Add(txtTelefon);

            y += 40;
            panelForm.Controls.Add(MakeLabel("Email *:", 0, y));
            txtEmail.Left = 100; txtEmail.Top = y; txtEmail.Width = 190; panelForm.Controls.Add(txtEmail);

            y += 60;
            btnAdauga.Text = "Adauga"; btnAdauga.Left = 0; btnAdauga.Top = y; btnAdauga.Width = 90;
            btnAdauga.BackColor = Color.SteelBlue; btnAdauga.ForeColor = Color.White;
            panelForm.Controls.Add(btnAdauga);

            btnModifica.Text = "Modifica"; btnModifica.Left = 100; btnModifica.Top = y; btnModifica.Width = 90;
            btnModifica.BackColor = Color.DarkOrange; btnModifica.ForeColor = Color.White;
            panelForm.Controls.Add(btnModifica);

            btnSterge.Text = "Sterge"; btnSterge.Left = 200; btnSterge.Top = y; btnSterge.Width = 90;
            btnSterge.BackColor = Color.Crimson; btnSterge.ForeColor = Color.White;
            panelForm.Controls.Add(btnSterge);

            this.Controls.AddRange(new Control[] { panelLista, panelForm });

            // ---- Events ----
            grid.SelectionChanged += Grid_SelectionChanged;
            btnAdauga.Click += BtnAdauga_Click;
            btnModifica.Click += BtnModifica_Click;
            btnSterge.Click += BtnSterge_Click;
            btnCauta.Click += (s, e) => LoadData(txtCautare.Text);
            btnRefresh.Click += (s, e) => { txtCautare.Clear(); LoadData(); };
            txtCautare.KeyPress += (s, e) => { if (e.KeyChar == (char)13) LoadData(txtCautare.Text); };
        }

        private Label MakeLabel(string text, int x, int y) =>
            new Label { Text = text, Left = x, Top = y + 3, Width = 95 };

        private void LoadData(string cautare = "")
        {
            try
            {
                // Dezabonam evenimentul ca sa nu se declanseze in timpul incarcarii
                grid.SelectionChanged -= Grid_SelectionChanged;

                var lista = CursantService.GetAll(cautare);
                grid.DataSource = lista.Select(c => new
                {
                    c.IdCursant,
                    c.Nume,
                    c.Prenume,
                    c.Telefon,
                    c.Email
                }).ToList();

                if (grid.Columns.Contains("IdCursant"))
                {
                    grid.Columns["IdCursant"].HeaderText = "ID";
                    grid.Columns["IdCursant"].Width = 40;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la incarcare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Reabornam evenimentul dupa ce datele sunt incarcate
                grid.SelectionChanged += Grid_SelectionChanged;
            }
        }

        private void Grid_SelectionChanged(object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            var row = grid.SelectedRows[0];
            if (row.Cells["IdCursant"]?.Value == null) return;
            try
            {
                selectedId = (int)row.Cells["IdCursant"].Value;
                txtNume.Text = row.Cells["Nume"].Value?.ToString() ?? "";
                txtPrenume.Text = row.Cells["Prenume"].Value?.ToString() ?? "";
                txtTelefon.Text = row.Cells["Telefon"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
            }
            catch { }
        }

        private bool Valideaza()
        {
            if (string.IsNullOrWhiteSpace(txtNume.Text) ||
                string.IsNullOrWhiteSpace(txtPrenume.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Campurile Nume, Prenume si Email sunt obligatorii!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Formatul email-ului este invalid!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtTelefon.Text) &&
                !Regex.IsMatch(txtTelefon.Text, @"^07\d{8}$"))
            {
                MessageBox.Show("Telefonul trebuie sa fie de forma 07XXXXXXXX!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private Cursant GetFromForm() => new Cursant
        {
            IdCursant = selectedId,
            Nume = txtNume.Text.Trim(),
            Prenume = txtPrenume.Text.Trim(),
            Telefon = txtTelefon.Text.Trim(),
            Email = txtEmail.Text.Trim()
        };

        private void BtnAdauga_Click(object? sender, EventArgs e)
        {
            if (!Valideaza()) return;
            if (CursantService.EmailExista(txtEmail.Text.Trim()))
            {
                MessageBox.Show("Exista deja un cursant cu acest email!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                CursantService.Adauga(GetFromForm());
                LoadData();
                ClearForm();
                MessageBox.Show("Cursant adaugat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnModifica_Click(object? sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Selectati un cursant din lista!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!Valideaza()) return;
            if (CursantService.EmailExista(txtEmail.Text.Trim(), selectedId))
            {
                MessageBox.Show("Exista deja un alt cursant cu acest email!", "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                CursantService.Modifica(GetFromForm());
                LoadData();
                MessageBox.Show("Cursant modificat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnSterge_Click(object? sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Selectati un cursant!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var rez = MessageBox.Show($"Stergeti cursantul si toate inscrierile sale?", "Confirmare",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rez == DialogResult.Yes)
            {
                try
                {
                    CursantService.Sterge(selectedId);
                    ClearForm();
                    LoadData();
                    MessageBox.Show("Cursant sters!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void ClearForm()
        {
            selectedId = 0;
            txtNume.Clear(); txtPrenume.Clear(); txtTelefon.Clear(); txtEmail.Clear();
        }
    }
}