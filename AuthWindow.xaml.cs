using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MetryxWPF
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private bool _passwordChangeMode = false;
        public AuthWindow()
        {
            InitializeComponent();
        }
        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            if(!FieldValidation())
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            using (PostgresContext db = new PostgresContext())
            {
                var user = db.Users
                       .FirstOrDefault(u => u.Username == Login.Text);


                if (user == null ||
                    !BCrypt.Net.BCrypt.Verify(
                        Password.Password,
                        user.Passwordhash))
                {
                    MessageBox.Show("Неверный логин или пароль");
                    return;
                }

                Session.CurrentUser = new UserSession
                {
                    UserId = user.Id,
                    FullName = user.Fullname,
                    RoleId = user.Roleid
                };
                if (user.IsThrowPassword)
                {
                    if (!_passwordChangeMode)
                    {
                        ThrowPassword.Visibility = Visibility.Visible;
                        _passwordChangeMode = true;
                        return;
                    }
                    if (!NewPasswordValidation())
                    {
                        MessageBox.Show("Пароли не совпадают");
                        return;
                    }
                    user.Passwordhash =
                        BCrypt.Net.BCrypt.HashPassword(NewPassword.Password);
                    user.IsThrowPassword = false;
                    db.SaveChanges();
                }

                MainWindow main = new MainWindow();
                main.Show();
                Close();
            }
        }
        private bool FieldValidation()
        {
            if(string.IsNullOrWhiteSpace(Login.Text) || string.IsNullOrWhiteSpace(Password.Password)) { return false; }
            return true;
        }
        private bool NewPasswordValidation()
        {
            if (string.IsNullOrEmpty(NewPassword.Password) || string.IsNullOrEmpty(RetypeNewPassword.Password) ||
                NewPassword.Password != RetypeNewPassword.Password)
                return false;
            return true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
