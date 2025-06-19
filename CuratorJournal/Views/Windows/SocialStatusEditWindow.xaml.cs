using CuratorJournal.DataBase;
using CuratorJournal.Helpers;
using CuratorJournal.Models;
using CuratorJournal.Session;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Windows
{
    /// <summary>
    /// Interaction logic for SocialStatusEditWindow.xaml
    /// </summary>
    public partial class SocialStatusEditWindow : Window
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private readonly StudentSocialView _currentRecord;
        private readonly int _groupId;

        public bool IsSaved { get; private set; } = false;

        public SocialStatusEditWindow(int groupId, StudentSocialView record = null)
        {
            InitializeComponent();
            _groupId = groupId;
            _currentRecord = record;

            LoadStudents();
            LoadSocialStatuses();

            if (_currentRecord != null)
            {
                // Режим редактирования
                StudentComboBox.SelectedValue = _currentRecord.StudentId;
                StatusComboBox.SelectedValue = _currentRecord.SocialStatusId;
            }
        }

        private void LoadStudents()
        {
            try
            {
                var students = _db.Student
                    .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _groupId))
                    .ToList()
                    .Select(s => new StudentSocialView
                    {
                        StudentId = s.StudentId,
                        StudentFullName = $"{s.Person.Surname} {s.Person.Name} {s.Person.Patronymic}"
                    })
                    .ToList();

                StudentComboBox.ItemsSource = students;
                StudentComboBox.SelectedValuePath = "StudentId";

                if (students.Any() && _currentRecord == null)
                {
                    StudentComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки студентов: {ex.Message}");
            }
        }

        private void LoadSocialStatuses()
        {
            try
            {
                StatusComboBox.ItemsSource = _db.SocialStatus.ToList();
                StatusComboBox.DisplayMemberPath = "Name";
                StatusComboBox.SelectedValuePath = "SocialStatusId";

                if (_db.SocialStatus.Any() && _currentRecord == null)
                {
                    StatusComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки социальных статусов: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StudentComboBox.SelectedItem == null || StatusComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите студента и социальный статус");
                    return;
                }

                var studentId = ((StudentSocialView)StudentComboBox.SelectedItem).StudentId;
                var statusId = ((SocialStatus)StatusComboBox.SelectedItem).SocialStatusId;

                var student = _db.Student
                    .Include(s => s.SocialStatus)
                    .FirstOrDefault(s => s.StudentId == studentId);

                var status = _db.SocialStatus.Find(statusId);

                if (student == null || status == null)
                {
                    MessageBox.Show("Ошибка: объект не найден");
                    return;
                }

                if (_currentRecord != null)
                {
                    // Режим редактирования - удаляем старый статус, добавляем новый
                    var oldStatus = student.SocialStatus.FirstOrDefault(ss => ss.SocialStatusId == _currentRecord.SocialStatusId);
                    if (oldStatus != null)
                    {
                        student.SocialStatus.Remove(oldStatus);
                    }
                }

                // Добавляем новый статус
                if (!student.SocialStatus.Contains(status))
                {
                    student.SocialStatus.Add(status);
                }

                _db.SaveChanges();
                IsSaved = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

