using System;
using System.Windows.Forms;

namespace _59219_LoginSystem
{
    public partial class LoginForm : Form
    {
        private int failedAttempts = 0;
        private const int MaxAttempts = 3;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblStatus.Text = "Please enter both username and password.";
                return;
            }

            try
            {
                string fullName = DatabaseHelper.ValidateLogin(username, password);

                if (fullName != null)
                {
                    failedAttempts = 0;
                    LoginSuccess(fullName);
                }
                else
                {
                    LoginFailed();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginSuccess(string fullName)
        {
            HomeForm homeForm = new HomeForm(fullName);
            homeForm.FormClosed += (s, args) =>
            {
                ClearForm();
                this.Show();
            };
            this.Hide();
            homeForm.Show();
        }

        private void LoginFailed()
        {
            failedAttempts++;
            int remaining = MaxAttempts - failedAttempts;

            if (remaining <= 0)
            {
                btnLogin.Enabled = false;
                lblStatus.Text = "Too many failed attempts. Login disabled.";
            }
            else
            {
                lblStatus.Text = "Invalid username or password. " + remaining + " attempt(s) left.";
            }

            txtPassword.Clear();
            txtPassword.Focus();
        }

        private void linkGoToRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            RegisterForm registerForm = new RegisterForm();
            registerForm.FormClosed += (s, args) => this.Close();
            registerForm.Show();
        }

        public void ClearForm()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            lblStatus.Text = "";
            txtUsername.Focus();
        }
    }
}