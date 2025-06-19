using CuratorJournal.DataBase;
using CuratorJournal.Session;
using CuratorJournal.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class ClassHourPage : Page
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private StudyGroup _currentGroup;

        public ClassHourPage()
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

        private void LoadClassHours()
        {
            if (_currentGroup == null) return;

            var classHours = _db.ClassHour
                .Include("StudentClassHour.Student")
                .Where(ch => ch.StudentClassHour.Any(sch =>
                    sch.Student.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId)))
                .Select(ch => new
                {
                    ch.ClassHourId,
                    ch.Date,
                    ch.Topic,
                    ch.Decision,
                    PresentCount = ch.StudentClassHour.Count(sch => sch.IsVisited)
                })
                .ToList()
                .Select(ch => new ClassHourView
                {
                    ClassHourId = ch.ClassHourId,
                    Date = ch.Date.ToShortDateString(),
                    Topic = ch.Topic,
                    Decision = ch.Decision,
                    PresentCount = ch.PresentCount
                })
                .ToList();

            MainDataGrid.ItemsSource = classHours;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу");
                return;
            }

            var addWindow = new ClassHourEditWindow(_currentGroup.StudyGroupId)
            {
                Owner = Window.GetWindow(this)
            };
            addWindow.ShowDialog();
            if (addWindow.IsSaved)
                LoadClassHours();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ClassHourView ch)
            {
                var editWindow = new ClassHourEditWindow(ch.ClassHourId, 0)
                {
                    Owner = Window.GetWindow(this)
                };
                editWindow.ShowDialog();
                if (editWindow.IsSaved)
                    LoadClassHours();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ClassHourView ch)
            {
                try
                {
                    var result = MessageBox.Show($"Удалить классный час \"{ch.Topic}\"?", "Подтверждение удаления",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) return;

                    var dbCh = _db.ClassHour
                        .Include("StudentClassHour") // Явно загружаем связанные данные
                        .FirstOrDefault(c => c.ClassHourId == ch.ClassHourId);

                    if (dbCh != null)
                    {
                        // Удаляем все связанные записи из StudentClassHour
                        _db.StudentClassHour.RemoveRange(dbCh.StudentClassHour);

                        // Теперь можно безопасно удалить сам классный час
                        _db.ClassHour.Remove(dbCh);

                        _db.SaveChanges();
                        LoadClassHours();
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
                LoadClassHours();
            }
            else
            {
                _currentGroup = null;
                MainDataGrid.ItemsSource = null;
            }
        }
    }

    public class ClassHourView
    {
        public int ClassHourId { get; set; }
        public string Date { get; set; }
        public string Topic { get; set; }
        public string Decision { get; set; }
        public int PresentCount { get; set; }
    }
}