using CuratorJournal.DataBase;
using CuratorJournal.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class TeacherCategoryReferencePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();

        public TeacherCategoryReferencePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MainDataGrid.ItemsSource = _db.TeacherCategory.ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            TeacherCategory selectedCategory = (sender as Button).DataContext as TeacherCategory;
            new TeacherCategoryAddEditWindow(selectedCategory).ShowDialog();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            TeacherCategory selectedCategory = (sender as Button).DataContext as TeacherCategory;
            MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите удалить категорию '{selectedCategory.Name}'?",
                "Успех!", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;

            try
            {
                _db.TeacherCategory.Remove(selectedCategory);
                _db.SaveChanges();
                MessageBox.Show("Категория успешно удалена", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch
            {
                MessageBox.Show("Возникла проблема при удалении!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new TeacherCategoryAddEditWindow().ShowDialog();
            LoadData();
        }
    }
}