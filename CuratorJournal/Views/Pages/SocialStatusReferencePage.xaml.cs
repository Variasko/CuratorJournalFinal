using CuratorJournal.DataBase;
using CuratorJournal.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class SocialStatusReferencePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();

        public SocialStatusReferencePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MainDataGrid.ItemsSource = _db.SocialStatus.ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SocialStatus selectedStatus = (sender as Button).DataContext as SocialStatus;
            new SocialStatusAddEditWindow(selectedStatus).ShowDialog();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            SocialStatus selectedStatus = (sender as Button).DataContext as SocialStatus;
            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить социальный статус '{selectedStatus.Name}'?",
                "Успех!", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;

            try
            {
                _db.SocialStatus.Remove(selectedStatus);
                _db.SaveChanges();
                MessageBox.Show("Социальный статус успешно удален", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch
            {
                MessageBox.Show("Возникла проблема при удалении!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new SocialStatusAddEditWindow().ShowDialog();
            LoadData();
        }
    }
}