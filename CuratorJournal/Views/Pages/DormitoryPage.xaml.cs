using CuratorJournal.DataBase;
using CuratorJournal.Models;
using CuratorJournal.Session;
using CuratorJournal.Views.Windows;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class DormitoryPage : Page
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private StudyGroup _currentGroup;

        public DormitoryPage()
        {
            InitializeComponent();
            LoadGroups();
        }

        private void LoadGroups()
        {
            try
            {
                GroupsComboBox.ItemsSource = _db.StudyGroup
                    .Where(sg => sg.CuratorId == CurrentSession.CurrentCurator.CuratorId)
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

        private void LoadStudents()
        {
            try
            {
                if (_currentGroup == null) return;

                var students = new List<StudentDormitoryView>();

                // Загружаем студентов с информацией об общежитии
                var dormStudents = _db.Student
                    .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId) &&
                                s.StudentInDormitory != null) // Изменено с .Any() на проверку null
                    .Include(s => s.Person)
                    .Include(s => s.StudentInDormitory) // Добавлено включение связи
                    .ToList();

                foreach (var student in dormStudents)
                {
                    students.Add(new StudentDormitoryView
                    {
                        StudentId = student.StudentId,
                        StudentFullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}",
                        Room = student.StudentInDormitory.Room // Прямой доступ к свойству
                    });
                }

                MainDataGrid.ItemsSource = students;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки студентов: {ex.Message}");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is StudentDormitoryView record)
            {
                var editWindow = new DormitoryEditWindow(record)
                {
                    Owner = Window.GetWindow(this)
                };

                editWindow.ShowDialog();

                if (editWindow.IsSaved)
                {
                    LoadStudents();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is StudentDormitoryView student)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Удалить информацию о проживании студента {student.StudentFullName}?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes) return;

                    var dbStudent = _db.Student
                        .Include(s => s.StudentInDormitory) // Явная загрузка связи
                        .FirstOrDefault(s => s.StudentId == student.StudentId);

                    if (dbStudent != null && dbStudent.StudentInDormitory != null)
                    {
                        _db.StudentInDormitory.Remove(dbStudent.StudentInDormitory);
                        _db.SaveChanges();
                        LoadStudents();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу");
                return;
            }

            var addWindow = new DormitoryEditWindow(_currentGroup.StudyGroupId)
            {
                Owner = Window.GetWindow(this)
            };

            addWindow.ShowDialog();

            if (addWindow.IsSaved)
            {
                LoadStudents();
            }
        }

        private void GroupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupsComboBox.SelectedItem is StudyGroup selectedGroup)
            {
                _currentGroup = selectedGroup;
                LoadStudents();
            }
            else
            {
                _currentGroup = null;
                MainDataGrid.ItemsSource = null;
            }
        }
    }
}