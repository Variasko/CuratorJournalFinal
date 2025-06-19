using CuratorJournal.DataBase;
using CuratorJournal.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class StudyGroupReferencePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();

        public StudyGroupReferencePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MainDataGrid.ItemsSource = _db.StudyGroup.Include("Specification").Include("Qualification").ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            StudyGroup selectedGroup = (sender as Button).DataContext as StudyGroup;
            new StudyGroupAddEditWindow(selectedGroup).ShowDialog();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            StudyGroup selectedGroup = (sender as Button).DataContext as StudyGroup;
            MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите удалить группу '{selectedGroup.FullName}'?",
                "Успех!", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;

            try
            {
                _db.StudyGroup.Remove(selectedGroup);
                _db.SaveChanges();
                MessageBox.Show("Группа успешно удалена", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch
            {
                MessageBox.Show("Возникла проблема при удалении!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new StudyGroupAddEditWindow().ShowDialog();
            LoadData();
        }
    }
}