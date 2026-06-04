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
using BCrypt.Net;

namespace MetryxWPF
{
    public partial class UserWindow : Window
    {
        public UserWindow(User user)
        {
            InitializeComponent();

            DataContext = user;

            using (PostgresContext db = new PostgresContext())
            {
                var roles = db.Roles.ToList();

                Role.ItemsSource = roles;
            }
            Role.SelectedValue = user.Roleid;
        }
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastName.Text) || string.IsNullOrWhiteSpace(FirstName.Text) ||
               string.IsNullOrWhiteSpace(Phonenumber.Text) || Role.SelectedValue == null ||
               string.IsNullOrWhiteSpace(Username.Text))
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }
            if(!PhonenumberValidation(Phonenumber.Text))
            {
                MessageBox.Show("Некорректный формат номера");
                return;
            }

            var user = DataContext as User;


            using (PostgresContext db = new PostgresContext())
            {
                if (user.Id == 0)
                {
                    if (!UsernameValidation(Username.Text))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует");
                        return;
                    }
                    if(!(Password.Password.Length > 1))
                    {
                        MessageBox.Show("Заполните пароль");
                        return;
                    }
                    db.Users.Add(InsertData(user));
                    db.SaveChanges();
                }
                else
                {
                    var existingUser = db.Users
                        .First(u => u.Id == user.Id);
                    if (!UsernameValidation(Username.Text) && existingUser.Username != Username.Text)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует");
                        return;
                    }

                    InsertData(existingUser);
                    db.SaveChanges();
                }
            }
        }
        public void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var deleteWindow = new ConfirmationWindow();
            if (deleteWindow.ShowDialog() == true)
            {
                var user = DataContext as User;
                using (PostgresContext db = new PostgresContext())
                {
                    var deletingUser = db.Users.First(d => d.Id == user.Id);

                    db.Users.Remove(deletingUser);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Невозможно удалить данного пользователя т.к. существуют зависимые записи");
                        return;
                    }
                    Close();
                }
            }
            else return;
        }
        private User InsertData(User user)
        {
            user.Lastname = LastName.Text;
            user.Firstname = FirstName.Text;
            user.Middlename = MiddleName.Text;
            user.Username = Username.Text;
            user.Phonenumber = Phonenumber.Text;
            user.Roleid = (int)Role.SelectedValue;
            user.Fullname = LastName.Text + " " + FirstName.Text + " " + MiddleName.Text;
            user.IsThrowPassword = ThrowPassword.IsChecked.Value;
            if(!string.IsNullOrWhiteSpace(Password.Password))
                user.Passwordhash = BCrypt.Net.BCrypt.HashPassword(Password.Password);

            return user;
        }
        private bool PhonenumberValidation(string phoneNumber)
        {
            var length = phoneNumber.Length;
            var firstChar = phoneNumber[0];

            if (length == 11 && (firstChar == '7' || firstChar == '8'))
                return true;
            else
                return false;
        }
        private bool UsernameValidation(string username)
        {
            using (PostgresContext db = new PostgresContext())
            {
                var user = db.Users.FirstOrDefault(x => x.Username == username);
                if (user != null) return false;
                else return true;
            }
        }
    }
}
