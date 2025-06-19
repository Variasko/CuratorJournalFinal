using CuratorJournal.DataBase;
using CuratorJournal.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class ParentReferencePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();

        public ParentReferencePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MainDataGrid.ItemsSource = _db.Parent.ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Parent selectedParent = (sender as Button).DataContext as Parent;
            new ParentAddEditWindow(selectedParent).ShowDialog();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Parent selectedParent = (sender as Button).DataContext as Parent;
            MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите удалить родителя '{selectedParent.Surname} {selectedParent.Name}'?",
                "Успех!", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;

            try
            {
                // Удаляем связи, если они есть
                var studentLinks = _db.Student.Where(s => s.Parent.Any(p => p.ParentId == selectedParent.ParentId)).ToList();
                foreach (var student in studentLinks)
                {
                    var links = student.Parent.Where(p => p.ParentId == selectedParent.ParentId).ToList();
                    foreach (var link in links)
                    {
                        student.Parent.Remove(link);
                    }
                }

                _db.Parent.Remove(selectedParent);
                _db.SaveChanges();
                MessageBox.Show("Родитель успешно удален", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new ParentAddEditWindow().ShowDialog();
            LoadData();
        }
    }
}