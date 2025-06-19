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
    public partial class ParentMeetingsPage : Page
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private StudyGroup _currentGroup;

        public ParentMeetingsPage()
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

        private void LoadMeetings()
        {
            try
            {
                if (_currentGroup == null) return;

                var meetings = _db.ParentMeeting
                    .Where(pm => pm.StudyGroupId == _currentGroup.StudyGroupId)
                    .Select(pm => new ParentMeetingView
                    {
                        MeetingId = pm.MeetingId,
                        Topic = pm.Topic,
                        Decision = pm.Decision,
                        Visited = pm.Visited,
                        NotVisitedWithReason = pm.NotVisitedWithReason,
                        NotVisited = pm.NotVisited
                    })
                    .ToList();

                MainDataGrid.ItemsSource = meetings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки протоколов: {ex.Message}");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ParentMeetingView record && _currentGroup != null)
            {
                var editWindow = new ParentMeetingEditWindow(_currentGroup.StudyGroupId, record)
                {
                    Owner = Window.GetWindow(this)
                };

                editWindow.ShowDialog();

                if (editWindow.IsSaved)
                {
                    LoadMeetings();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ParentMeetingView meeting)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Удалить протокол собрания '{meeting.Topic}'?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes) return;

                    var meetingToDelete = _db.ParentMeeting.Find(meeting.MeetingId);
                    if (meetingToDelete != null)
                    {
                        _db.ParentMeeting.Remove(meetingToDelete);
                        _db.SaveChanges();
                        LoadMeetings();
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

            var addWindow = new ParentMeetingEditWindow(_currentGroup.StudyGroupId)
            {
                Owner = Window.GetWindow(this)
            };

            addWindow.ShowDialog();

            if (addWindow.IsSaved)
            {
                LoadMeetings();
            }
        }

        private void GroupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupsComboBox.SelectedItem is StudyGroup selectedGroup)
            {
                _currentGroup = selectedGroup;
                LoadMeetings();
            }
            else
            {
                _currentGroup = null;
                MainDataGrid.ItemsSource = null;
            }
        }
    }
}