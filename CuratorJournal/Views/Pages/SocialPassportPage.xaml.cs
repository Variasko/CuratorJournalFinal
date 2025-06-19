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
    public partial class SocialPassportPage : Page
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private StudyGroup _currentGroup;

        public SocialPassportPage()
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

                var students = _db.Student
                    .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId))
                    .Include(s => s.Person)
                    .Include(s => s.SocialStatus)
                    .ToList();

                var socialStatuses = new List<StudentSocialView>();

                foreach (var student in students)
                {
                    foreach (var status in student.SocialStatus)
                    {
                        socialStatuses.Add(new StudentSocialView
                        {
                            StudentId = student.StudentId,
                            StudentFullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}",
                            SocialStatusId = status.SocialStatusId,
                            SocialStatusName = status.Name
                        });
                    }
                }

                MainDataGrid.ItemsSource = socialStatuses;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки студентов: {ex.Message}");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is StudentSocialView record && _currentGroup != null)
            {
                var editWindow = new SocialStatusEditWindow(_currentGroup.StudyGroupId, record)
                {
                    Owner = Window.GetWindow(this)
                };

                editWindow.ShowDialog();

                if (editWindow.IsSaved)
                {
                    LoadStudents(); // Обновляем данные после закрытия окна
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is StudentSocialView studentSocial)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Удалить социальный статус '{studentSocial.SocialStatusName}' у студента {studentSocial.StudentFullName}?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes) return;

                    var student = _db.Student
                        .Include(s => s.SocialStatus)
                        .FirstOrDefault(s => s.StudentId == studentSocial.StudentId);

                    if (student == null)
                    {
                        MessageBox.Show("Студент не найден");
                        return;
                    }

                    var statusToRemove = student.SocialStatus
                        .FirstOrDefault(ss => ss.SocialStatusId == studentSocial.SocialStatusId);

                    if (statusToRemove != null)
                    {
                        student.SocialStatus.Remove(statusToRemove);
                        _db.SaveChanges();
                        LoadStudents(); // Обновляем данные
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

            var addWindow = new SocialStatusEditWindow(_currentGroup.StudyGroupId, null)
            {
                Owner = Window.GetWindow(this)
            };

            addWindow.ShowDialog();

            if (addWindow.IsSaved)
            {
                LoadStudents(); // Обновляем данные после закрытия окна
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