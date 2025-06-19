using CuratorJournal.DataBase;
using CuratorJournal.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class CuratorReferencePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();

        public CuratorReferencePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MainDataGrid.ItemsSource = _db.Curator.Include("Person").Include("TeacherCategory").Include("User").ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Curator selectedCurator = (sender as Button).DataContext as Curator;
            new CuratorAddEditWindow(selectedCurator).ShowDialog();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Curator selectedCurator = (sender as Button).DataContext as Curator;
            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить куратора '{selectedCurator.Person.Surname} {selectedCurator.Person.Name}'?",
                "Успех!", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;

            try
            {
                var person = _db.Person.Find(selectedCurator.PersonId);
                var user = _db.User.Find(selectedCurator.UserId);

                // Удаляем связи
                _db.Curator.Remove(selectedCurator);
                _db.SaveChanges();

                // Можно оставить пользователя и персону, если они используются где-то ещё
                // Или удалить их тоже:
                // _db.Person.Remove(person);
                // _db.User.Remove(user);
                // _db.SaveChanges();

                MessageBox.Show("Куратор успешно удален", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new CuratorAddEditWindow().ShowDialog();
            LoadData();
        }
    }
}