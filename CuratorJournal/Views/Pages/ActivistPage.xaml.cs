using CuratorJournal.DataBase;
using CuratorJournal.Session;
using CuratorJournal.Views.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class ActivistPage : Page
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private StudyGroup _currentGroup;

        public ActivistPage()
        {
            InitializeComponent();
            LoadGroups();
        }

        private void LoadGroups()
        {
            try
            {
                GroupsComboBox.ItemsSource = _db.StudyGroup
                    .Where(g => g.CuratorId == CurrentSession.CurrentCurator.CuratorId)
                    .ToList();

                GroupsComboBox.DisplayMemberPath = "FullName";
                GroupsComboBox.SelectedValuePath = "StudyGroupId";

                if (GroupsComboBox.Items.Count > 0)
                    GroupsComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групп: {ex.Message}");
            }
        }

        private void LoadData()
        {
            if (_currentGroup == null) return;

            // Сначала получаем данные без форматирования строк
            var query = _db.Student
                .Include("Person")
                .Include("GroupPost")
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId) &&
                            s.GroupPost.Any()) // Только студенты с назначением
                .Select(s => new
                {
                    s.StudentId,
                    s.Person.Surname,
                    s.Person.Name,
                    s.Person.Patronymic,
                    PostName = s.GroupPost.FirstOrDefault().Name
                })
                .ToList(); // <<< Выполняем запрос к БД

            // Теперь форматируем ФИО в C# (не в LINQ-to-Entities)
            var viewList = query.Select(x => new ActivistView
            {
                StudentId = x.StudentId,
                StudentFullName = $"{x.Surname} {x.Name} {x.Patronymic}",
                PostName = x.PostName ?? "—"
            }).ToList();

            MainDataGrid.ItemsSource = viewList;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу");
                return;
            }

            var addWindow = new ActivistEditWindow(_currentGroup.StudyGroupId)
            {
                Owner = Window.GetWindow(this)
            };
            addWindow.ShowDialog();
            if (addWindow.IsSaved)
                LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ActivistView record)
            {
                var editWindow = new ActivistEditWindow(record.StudentId, record.PostName)
                {
                    Owner = Window.GetWindow(this)
                };
                editWindow.ShowDialog();
                if (editWindow.IsSaved)
                    LoadData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ActivistView record)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Удалить должность \"{record.PostName}\" у студента {record.StudentFullName}?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes) return;

                    var student = _db.Student
                        .Include("GroupPost")
                        .FirstOrDefault(s => s.StudentId == record.StudentId);

                    var post = student?.GroupPost.FirstOrDefault();
                    if (post != null)
                    {
                        student.GroupPost.Remove(post);
                        _db.SaveChanges();
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void GroupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupsComboBox.SelectedItem is StudyGroup selectedGroup)
            {
                _currentGroup = selectedGroup;
                LoadData();
            }
            else
            {
                _currentGroup = null;
                MainDataGrid.ItemsSource = null;
            }
        }
    }

    public class ActivistView
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; }
        public string PostName { get; set; }
    }
}