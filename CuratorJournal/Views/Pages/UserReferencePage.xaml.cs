using CuratorJournal.DataBase;
using CuratorJournal.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class UserReferencePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();

        public UserReferencePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MainDataGrid.ItemsSource = _db.User.ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = (sender as Button).DataContext as User;
            new UserAddEditWindow(selectedUser).ShowDialog();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = (sender as Button).DataContext as User;
            MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите удалить пользователя '{selectedUser.Login}'?",
                "Успех!", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;

            try
            {
                _db.User.Remove(selectedUser);
                _db.SaveChanges();
                MessageBox.Show("Пользователь успешно удален", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch
            {
                MessageBox.Show("Возникла проблема при удалении!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new UserAddEditWindow().ShowDialog();
            LoadData();
        }
    }
}