using CuratorJournal.DataBase;
using CuratorJournal.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class SpecificationReferencePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();

        public SpecificationReferencePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MainDataGrid.ItemsSource = _db.Specification.ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Specification selectedSpec = (sender as Button).DataContext as Specification;
            new SpecificationAddEditWindow(selectedSpec).ShowDialog();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Specification selectedSpec = (sender as Button).DataContext as Specification;
            MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите удалить спецификацию '{selectedSpec.Name}'?",
                "Успех!", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;

            try
            {
                _db.Specification.Remove(selectedSpec);
                _db.SaveChanges();
                MessageBox.Show("Спецификация успешно удалена", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch
            {
                MessageBox.Show("Возникла проблема при удалении!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new SpecificationAddEditWindow().ShowDialog();
            LoadData();
        }
    }
}