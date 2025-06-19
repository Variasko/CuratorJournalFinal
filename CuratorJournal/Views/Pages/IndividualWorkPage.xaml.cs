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
    public partial class IndividualWorkPage : Page
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private StudyGroup _currentGroup;

        public IndividualWorkPage()
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

            var works = _db.IndividualWork
                .Where(w => w.Student != null && w.Student.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId))
                .ToList();

            var viewList = new List<IndividualWorkView>();

            foreach (var work in works)
            {
                string personName = "";

                if (work.IsStudent && work.Student != null)
                {
                    personName = $"{work.Student.Person.Surname} {work.Student.Person.Name} {work.Student.Person.Patronymic}";
                }
                else if (!work.IsStudent && work.Parent != null)
                {
                    personName = $"{work.Parent.Surname} {work.Parent.Name} {work.Parent.Patronymic}";
                }

                viewList.Add(new IndividualWorkView
                {
                    IndividualWorkId = work.IndividualWorkId,
                    Date = work.Date.ToShortDateString(),
                    Topic = work.Topic,
                    Decision = work.Decision,
                    Person = personName
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

            var addWindow = new IndividualWorkEditWindow(_currentGroup.StudyGroupId)
            {
                Owner = Window.GetWindow(this)
            };
            addWindow.ShowDialog();
            if (addWindow.IsSaved)
                LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is IndividualWorkView record)
            {
                var editWindow = new IndividualWorkEditWindow(record.IndividualWorkId, 0)
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
            if (sender is Button button && button.DataContext is IndividualWorkView record)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Удалить запись о работе \"{record.Topic}\"?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes) return;

                    var dbWork = _db.IndividualWork.Find(record.IndividualWorkId);
                    if (dbWork != null)
                    {
                        _db.IndividualWork.Remove(dbWork);
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

    public class IndividualWorkView
    {
        public int IndividualWorkId { get; set; }
        public string Date { get; set; }
        public string Topic { get; set; }
        public string Decision { get; set; }
        public string Person { get; set; }
    }
}