using CuratorJournal.DataBase;
using System;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class HobbyAddEditWindow : Window
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        private Hobby _hobby;
        private bool _isEditing;

        public HobbyAddEditWindow(Hobby hobby)
        {
            InitializeComponent();
            _hobby = hobby;
            _isEditing = true;
            HobbyName.Text = hobby.Name;
        }

        public HobbyAddEditWindow()
        {
            InitializeComponent();
            _isEditing = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditing)
            {
                Hobby hobby = _db.Hobby.Find(_hobby.HobbyId);
                hobby.Name = HobbyName.Text;
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Хобби успешно сохранено!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Возникла проблема при редактировании!\n{ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                _db.Hobby.Add(new Hobby { Name = HobbyName.Text });
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Хобби успешно добавлено!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Возникла проблема при добавлении!\n{ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
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