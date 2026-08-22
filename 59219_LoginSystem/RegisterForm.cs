using System;
using System.Windows.Forms;

namespace _59219_LoginSystem
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            string email = txtEmail.Text.Trim();
            string fullName = txtFullName.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(fullName))
            {
                lblMessage.Text = "All fields are required.";
                return;
            }

            if (password.Length < 6)
            {
                lblMessage.Text = "Password must be at least 6 characters.";
                return;
            }

            if (password != confirmPassword)
            {
                lblMessage.Text = "Passwords do not match.";
                return;
            }

            if (!email.Contains("@"))
            {
                lblMessage.Text = "Please enter a valid email.";
                return;
            }

            try
            {
                if (DatabaseHelper.UsernameExists(username))
                {
                    lblMessage.Text = "Username already taken.";
                    return;
                }

                if (DatabaseHelper.RegisterUser(username, password, email, fullName))
                {
                    MessageBox.Show("Registration successful!", "Success",
                                     MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    this.Hide();
                    LoginForm loginForm = new LoginForm();
                    loginForm.FormClosed += (s, args) => this.Close();
                    loginForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkGoToLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.FormClosed += (s, args) => this.Close();
            loginForm.Show();
        }

        private void ClearForm()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            txtEmail.Clear();
            txtFullName.Clear();
            lblMessage.Text = "";
            txtUsername.Focus();
        }
    }
}