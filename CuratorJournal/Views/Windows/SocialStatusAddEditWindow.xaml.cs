using CuratorJournal.DataBase;
using System;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class SocialStatusAddEditWindow : Window
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        private SocialStatus _socialStatus;
        private bool _isEditing;

        public SocialStatusAddEditWindow(SocialStatus socialStatus)
        {
            InitializeComponent();
            _socialStatus = socialStatus;
            _isEditing = true;
            SocialStatusName.Text = socialStatus.Name;
        }

        public SocialStatusAddEditWindow()
        {
            InitializeComponent();
            _isEditing = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = SocialStatusName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Заполните поле", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditing)
            {
                SocialStatus status = _db.SocialStatus.Find(_socialStatus.SocialStatusId);
                status.Name = name;
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Социальный статус успешно обновлен!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
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
                _db.SocialStatus.Add(new SocialStatus { Name = name });
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Социальный статус успешно добавлен!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
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