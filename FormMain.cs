// Ziua 6 - imbunatatirea interfetei principale
// Ziua 21 - revizuirea si imbunatatirea interfetei FormMain
namespace CentruInstruire
{
    public class FormMain : Form
    {
        public FormMain()
        {
            InitUI();
        }

        private void InitUI()
        {
            this.Text = "Centru de Instruire - Sistem de Evidenta";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(600, 400);

            // Meniu
            var menuStrip = new MenuStrip();
            var menuCursanti = new ToolStripMenuItem("Cursanti");
            var menuCursuri = new ToolStripMenuItem("Cursuri");
            var menuInscrieri = new ToolStripMenuItem("Inscrieri");
            var menuRaport = new ToolStripMenuItem("Raport");

            menuCursanti.Click += (s, e) => new FormCursanti().Show();
            menuCursuri.Click += (s, e) => new FormCursuri().Show();
            menuInscrieri.Click += (s, e) => new FormInscrieri().Show();
            menuRaport.Click += (s, e) => new FormRaport().ShowDialog();

            menuStrip.Items.AddRange(new ToolStripItem[] { menuCursanti, menuCursuri, menuInscrieri, menuRaport });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Continut principal - butoane mari
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            var lblWelcome = new Label
            {
                Text = "Sistem de Evidenta Cursuri",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };

            var tableLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            tableLayout.Controls.Add(MakeButton("  Cursanti", Color.SteelBlue, () => new FormCursanti().Show()), 0, 0);
            tableLayout.Controls.Add(MakeButton("  Cursuri", Color.SeaGreen, () => new FormCursuri().Show()), 1, 0);
            tableLayout.Controls.Add(MakeButton("  Inscrieri", Color.DarkOrange, () => new FormInscrieri().Show()), 0, 1);
            tableLayout.Controls.Add(MakeButton("  Raport", Color.Purple, () => new FormRaport().ShowDialog()), 1, 1);

            panel.Controls.Add(tableLayout);
            panel.Controls.Add(lblWelcome);

            this.Controls.Add(panel);

            // Verificare conexiune la pornire
            if (!Database.TestConnection())
            {
                var result = MessageBox.Show(
                    "Nu s-a putut conecta la baza de date MySQL!\n\n" +
                    "Verificati ca MySQL ruleaza si parola este corecta.\n\n" +
                    "Doriti sa introduceti parola MySQL acum?",
                    "Eroare conexiune",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    string pwd = Microsoft.VisualBasic.Interaction.InputBox(
                        "Introduceti parola MySQL pentru user 'root':", "Parola MySQL", "");
                    Database.SetPassword(pwd);
                    if (!Database.TestConnection())
                        MessageBox.Show("Conexiunea a esuat din nou. Editati fisierul Database.cs manual.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show("Conectat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private Button MakeButton(string text, Color color, Action action)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                BackColor = color,
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                Margin = new Padding(8),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => action();
            return btn;
        }
    }
}