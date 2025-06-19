using CuratorJournal.DataBase;
using CuratorJournal.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class HobbyReferencePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();

        public HobbyReferencePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MainDataGrid.ItemsSource = _db.Hobby.ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Hobby selectedHobby = (sender as Button).DataContext as Hobby;
            new HobbyAddEditWindow(selectedHobby).ShowDialog();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Hobby selectedHobby = (sender as Button).DataContext as Hobby;
            MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите удалить хобби '{selectedHobby.Name}'?",
                "Успех!", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;

            try
            {
                _db.Hobby.Remove(selectedHobby);
                _db.SaveChanges();
                MessageBox.Show("Хобби успешно удалено", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch
            {
                MessageBox.Show("Возникла проблема при удалении!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new HobbyAddEditWindow().ShowDialog();
            LoadData();
        }
    }
}