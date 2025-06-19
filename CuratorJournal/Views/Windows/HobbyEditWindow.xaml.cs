using CuratorJournal.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Windows
{
    public partial class HobbyEditWindow : Window
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private bool _isEditMode = false;
        private int? _studentId;
        private int _groupId;

        public bool IsSaved { get; private set; } = false;

        // Конструктор для редактирования
        public HobbyEditWindow(int studentId, int a)
        {
            InitializeComponent();
            _studentId = studentId;
            _isEditMode = true;
            LoadData();
        }

        // Конструктор для добавления
        public HobbyEditWindow(int groupId)
        {
            InitializeComponent();
            _groupId = groupId;
            LoadForAddMode();
        }

        private void LoadForAddMode()
        {
            var students = _db.Student
                .Include("Person")
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _groupId))
                .ToList();

            var studentModels = students.Select(s => new StudentModel
            {
                StudentId = s.StudentId,
                StudentFullName = $"{s.Person.Surname} {s.Person.Name} {s.Person.Patronymic}"
            }).ToList();

            StudentComboBox.ItemsSource = studentModels;

            var hobbies = _db.Hobby.ToList();
            HobbiesList.ItemsSource = hobbies.Select(h => new HobbyModel
            {
                HobbyId = h.HobbyId,
                Name = h.Name,
                IsSelected = false
            }).ToList();
        }

        private void LoadData()
        {
            var student = _db.Student
                .Include("Person")
                .FirstOrDefault(s => s.StudentId == _studentId);

            if (student == null) return;

            int groupId = student.StudyGroup.First().StudyGroupId;

            var studentFullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}";

            var students = _db.Student
                .Include("Person")
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == groupId))
                .ToList();

            var studentModels = students.Select(s => new StudentModel
            {
                StudentId = s.StudentId,
                StudentFullName = $"{s.Person.Surname} {s.Person.Name} {s.Person.Patronymic}"
            }).ToList();

            StudentComboBox.ItemsSource = studentModels;
            StudentComboBox.SelectedValue = _studentId;

            var hobbies = _db.Hobby.ToList();
            var hobbyModels = hobbies.Select(h => new HobbyModel
            {
                HobbyId = h.HobbyId,
                Name = h.Name,
                IsSelected = student.Hobby.Any(sh => sh.HobbyId == h.HobbyId)
            }).ToList();

            HobbiesList.ItemsSource = hobbyModels;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StudentComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите студента");
                    return;
                }

                var selectedStudent = ((StudentModel)StudentComboBox.SelectedItem);
                var hobbies = (List<HobbyModel>)HobbiesList.ItemsSource;

                if (_isEditMode)
                {
                    var student = _db.Student
                        .Include("Hobby")
                        .FirstOrDefault(s => s.StudentId == selectedStudent.StudentId);

                    if (student == null) return;

                    var existingHobbies = student.Hobby.ToList();

                    foreach (var item in hobbies)
                    {
                        var dbHobby = _db.Hobby.Find(item.HobbyId);

                        if (item.IsSelected)
                        {
                            if (!student.Hobby.Any(h => h.HobbyId == dbHobby.HobbyId))
                            {
                                student.Hobby.Add(dbHobby);
                            }
                        }
                        else
                        {
                            var existing = student.Hobby.FirstOrDefault(h => h.HobbyId == dbHobby.HobbyId);
                            if (existing != null)
                            {
                                student.Hobby.Remove(existing);
                            }
                        }
                    }

                    _db.SaveChanges();
                }
                else
                {
                    var student = _db.Student
                        .Include("Hobby")
                        .FirstOrDefault(s => s.StudentId == selectedStudent.StudentId);

                    if (student == null) return;

                    foreach (var item in hobbies)
                    {
                        var dbHobby = _db.Hobby.Find(item.HobbyId);

                        if (item.IsSelected)
                        {
                            if (!student.Hobby.Any(h => h.HobbyId == dbHobby.HobbyId))
                            {
                                student.Hobby.Add(dbHobby);
                            }
                        }
                    }

                    _db.SaveChanges();
                }

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

    public class StudentModel
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; }
    }

    public class HobbyModel
    {
        public int HobbyId { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}