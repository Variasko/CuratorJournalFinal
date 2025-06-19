using CuratorJournal.DataBase;
using CuratorJournal.Models;
using CuratorJournal.Session;
using CuratorJournal.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class ExtracurricularPage : Page
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private StudyGroup _currentGroup;

        public ExtracurricularPage()
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

            var students = _db.Student
                .Include("Person")
                .Include("Hobby")
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId))
                .Where(s => s.Hobby.Any()) // Добавляем фильтр: только студенты с хобби
                .ToList();

            var viewList = new List<ExtracurricularView>();

            foreach (var student in students)
            {
                viewList.Add(new ExtracurricularView
                {
                    StudentId = student.StudentId,
                    StudentFullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}",
                    Hobbies = string.Join(", ", student.Hobby.Select(h => h.Name))
                });
            }

            MainDataGrid.ItemsSource = viewList;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу");
                return;
            }

            var addWindow = new HobbyEditWindow(_currentGroup.StudyGroupId)
            {
                Owner = Window.GetWindow(this)
            };
            addWindow.ShowDialog();
            if (addWindow.IsSaved)
                LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ExtracurricularView record)
            {
                var editWindow = new HobbyEditWindow(record.StudentId, 0)
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
            if (sender is Button button && button.DataContext is ExtracurricularView record)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Удалить все хобби студента {record.StudentFullName}?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes) return;

                    var student = _db.Student
                        .Include("Hobby")
                        .FirstOrDefault(s => s.StudentId == record.StudentId);

                    if (student != null)
                    {
                        student.Hobby.Clear();
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

    public class ExtracurricularView
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; }
        public string Hobbies { get; set; }
    }
}