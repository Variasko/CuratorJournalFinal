using CuratorJournal.DataBase;
using System;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class UserAddEditWindow : Window
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        private User _user;
        private bool _isEditing;

        public UserAddEditWindow(User user)
        {
            InitializeComponent();
            _user = user;
            _isEditing = true;
            LoginTextBox.Text = user.Login;
            PasswordBox.Password = user.Password;
            IsAdminCheckBox.IsChecked = user.IsAdmin;
        }

        public UserAddEditWindow()
        {
            InitializeComponent();
            _isEditing = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            bool isAdmin = IsAdminCheckBox.IsChecked ?? false;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditing)
            {
                User user = _db.User.Find(_user.UserId);
                user.Login = login;
                user.Password = password;
                user.IsAdmin = isAdmin;

                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Пользователь успешно обновлен!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения:\n{ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                _db.User.Add(new User { Login = login, Password = password, IsAdmin = isAdmin });
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Пользователь успешно добавлен!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка добавления:\n{ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}