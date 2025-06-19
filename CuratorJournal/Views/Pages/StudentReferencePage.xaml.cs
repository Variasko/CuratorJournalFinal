using CuratorJournal.DataBase;
using CuratorJournal.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class StudentReferencePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();

        public StudentReferencePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MainDataGrid.ItemsSource = _db.Student.Include("Person").Include("StudyGroup").ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = (sender as Button).DataContext as Student;
            new StudentAddEditWindow(selectedStudent).ShowDialog();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = (sender as Button).DataContext as Student;
            var result = MessageBox.Show($"Вы уверены, что хотите удалить студента {selectedStudent.Person.Surname} {selectedStudent.Person.Name}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                return;

            try
            {
                // Удаляем связи
                var parentLinks = _db.Parent.Where(p => p.Student.Any(s => s.StudentId == selectedStudent.StudentId)).ToList();
                foreach (var parent in parentLinks)
                {
                    var links = parent.Student.Where(s => s.StudentId == selectedStudent.StudentId).ToList();
                    foreach (var link in links)
                    {
                        parent.Student.Remove(link);
                    }
                }

                // Удаляем самого студента и персону
                var person = _db.Person.Find(selectedStudent.Person.PersonId);
                _db.Student.Remove(selectedStudent);
                _db.SaveChanges();

                // Опционально: можно удалить и Person, если она больше не используется
                // _db.Person.Remove(person);

                LoadData();
                MessageBox.Show("Студент успешно удален", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new StudentAddEditWindow().ShowDialog();
            LoadData();
        }
    }
}