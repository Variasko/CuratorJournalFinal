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
    public partial class CharacteristicPage : Page
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private StudyGroup _currentGroup;

        public CharacteristicPage()
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

            var studentsQuery = _db.Student
                .Include("Person")
                .Include("CuratorCharacteristic")
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId))
                .ToList();

            // Фильтруем только тех, у кого есть характеристика
            var studentsWithChar = studentsQuery
                .Where(s => s.CuratorCharacteristic != null && s.CuratorCharacteristic.Any())
                .ToList();

            var viewList = new List<CharacteristicView>();

            foreach (var student in studentsWithChar)
            {
                var lastChar = student.CuratorCharacteristic
                    .OrderByDescending(c => c.DateCreated)
                    .FirstOrDefault();

                string characteristicPreview = "";

                if (lastChar != null)
                {
                    characteristicPreview = lastChar.Characteristic.Length > 50
                        ? lastChar.Characteristic.Substring(0, 50) + "..."
                        : lastChar.Characteristic;
                }

                viewList.Add(new CharacteristicView
                {
                    StudentId = student.StudentId,
                    StudentFullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}",
                    CharacteristicPreview = characteristicPreview
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

            var addWindow = new CharacteristicEditWindow(_currentGroup.StudyGroupId)
            {
                Owner = Window.GetWindow(this)
            };
            addWindow.ShowDialog();
            if (addWindow.IsSaved)
                LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CharacteristicView record)
            {
                var editWindow = new CharacteristicEditWindow(record.StudentId, 0)
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
            if (sender is Button button && button.DataContext is CharacteristicView record)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Удалить характеристику студента {record.StudentFullName}?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes) return;

                    var charToDelete = _db.CuratorCharacteristic
                        .FirstOrDefault(c => c.StudentId == record.StudentId);

                    if (charToDelete != null)
                    {
                        _db.CuratorCharacteristic.Remove(charToDelete);
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

    public class CharacteristicView
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; }
        public string CharacteristicPreview { get; set; }
    }
}