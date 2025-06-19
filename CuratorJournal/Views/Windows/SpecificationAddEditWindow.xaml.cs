using CuratorJournal.DataBase;
using System;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class SpecificationAddEditWindow : Window
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        private Specification _specification;
        private bool _isEditing;

        public SpecificationAddEditWindow(Specification specification)
        {
            InitializeComponent();
            _specification = specification;
            _isEditing = true;
            AbbreviationTextBox.Text = specification.Abbreviation;
            NameTextBox.Text = specification.Name;
        }

        public SpecificationAddEditWindow()
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
                Specification spec = _db.Specification.Find(_specification.Abbreviation);
                spec.Abbreviation = abbreviation;
                spec.Name = name;
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Спецификация успешно обновлена!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
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
                _db.Specification.Add(new Specification { Abbreviation = abbreviation, Name = name });
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Спецификация успешно добавлена!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
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