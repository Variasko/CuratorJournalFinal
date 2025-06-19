using CuratorJournal.DataBase;
using CuratorJournal.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class QualificationReferencePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();

        public QualificationReferencePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MainDataGrid.ItemsSource = _db.Qualification.ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Qualification selectedQualification = (sender as Button).DataContext as Qualification;
            new QualificationAddEditWindow(selectedQualification).ShowDialog();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Qualification selectedQualification = (sender as Button).DataContext as Qualification;
            MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите удалить квалификацию '{selectedQualification.Name}'?",
                "Успех!", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;

            try
            {
                _db.Qualification.Remove(selectedQualification);
                _db.SaveChanges();
                MessageBox.Show("Квалификация успешно удалена", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch
            {
                MessageBox.Show("Возникла проблема при удалении!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new QualificationAddEditWindow().ShowDialog();
            LoadData();
        }
    }
}