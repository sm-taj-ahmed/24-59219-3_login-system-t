using System;
using System.Windows.Forms;

namespace _59219_LoginSystem
{
    public partial class HomeForm : Form
    {
        public HomeForm(string fullName)
        {
            InitializeComponent();
            lblWelcome.Text = "Welcome, " + fullName;
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void LoadUsers(string searchTerm = null)
        {
            try
            {
                dgvUsers.DataSource = DatabaseHelper.GetUsers(searchTerm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load users: " + ex.Message, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers(txtSearch.Text.Trim());
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadUsers();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}