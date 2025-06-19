using CuratorJournal.DataBase;
using System;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class QualificationAddEditWindow : Window
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        private Qualification _qualification;
        private bool _isEditing;

        public QualificationAddEditWindow(Qualification qualification)
        {
            InitializeComponent();
            _qualification = qualification;
            _isEditing = true;
            AbbreviationTextBox.Text = qualification.Abbreviation;
            NameTextBox.Text = qualification.Name;
        }

        public QualificationAddEditWindow()
        {
            InitializeComponent();
            _isEditing = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string abbreviation = AbbreviationTextBox.Text.Trim();
            string name = NameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(abbreviation) || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Заполните все поля", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditing)
            {
                Qualification qual = _db.Qualification.Find(_qualification.Abbreviation);
                qual.Abbreviation = abbreviation;
                qual.Name = name;
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Квалификация успешно обновлена!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
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
                _db.Qualification.Add(new Qualification { Abbreviation = abbreviation, Name = name });
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Квалификация успешно добавлена!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
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