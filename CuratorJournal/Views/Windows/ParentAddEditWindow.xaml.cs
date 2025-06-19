using CuratorJournal.DataBase;
using System;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class ParentAddEditWindow : Window
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        private Parent _parent;
        private bool _isEditing;

        public ParentAddEditWindow(Parent parent)
        {
            InitializeComponent();
            _parent = parent;
            _isEditing = true;

            SurnameTextBox.Text = parent.Surname;
            NameTextBox.Text = parent.Name;
            PatronymicTextBox.Text = parent.Patronymic;
            PhoneTextBox.Text = parent.Phone;
            EmailTextBox.Text = parent.Email;
        }

        public ParentAddEditWindow()
        {
            InitializeComponent();
            _isEditing = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string surname = SurnameTextBox.Text.Trim();
            string name = NameTextBox.Text.Trim();
            string patronymic = PatronymicTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Заполните обязательные поля: Фамилия и Имя", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditing)
            {
                Parent parent = _db.Parent.Find(_parent.ParentId);
                parent.Surname = surname;
                parent.Name = name;
                parent.Patronymic = patronymic;
                parent.Phone = phone;
                parent.Email = email;

                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Родитель успешно обновлён", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
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
                _db.Parent.Add(new Parent
                {
                    Surname = surname,
                    Name = name,
                    Patronymic = patronymic,
                    Phone = phone,
                    Email = email
                });

                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Родитель успешно добавлен", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
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