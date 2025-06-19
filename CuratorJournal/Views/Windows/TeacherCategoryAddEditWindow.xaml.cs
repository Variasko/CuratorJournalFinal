using CuratorJournal.DataBase;
using System;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class TeacherCategoryAddEditWindow : Window
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        private TeacherCategory _category;
        private bool _isEditing;

        public TeacherCategoryAddEditWindow(TeacherCategory category)
        {
            InitializeComponent();
            _category = category;
            _isEditing = true;
            CategoryName.Text = category.Name;
        }

        public TeacherCategoryAddEditWindow()
        {
            InitializeComponent();
            _isEditing = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = CategoryName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Заполните поле 'Название'", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditing)
            {
                TeacherCategory category = _db.TeacherCategory.Find(_category.CategoryId);
                category.Name = name;
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Категория успешно обновлена!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
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
                _db.TeacherCategory.Add(new TeacherCategory { Name = name });
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Категория успешно добавлена!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
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