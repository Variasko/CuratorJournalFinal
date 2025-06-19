using CuratorJournal.DataBase;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Windows
{
    public partial class ClassHourEditWindow : Window
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private int? _classHourId;
        private List<StudentAttendanceModel> _students = new List<StudentAttendanceModel>();
        private bool _isEditMode = false;
        private int _groupId;

        public bool IsSaved { get; private set; } = false;

        // Конструктор для редактирования
        public ClassHourEditWindow(int classHourId, int a)
        {
            InitializeComponent();
            _classHourId = classHourId;
            _isEditMode = true;
            LoadData();
        }

        // Конструктор для добавления
        public ClassHourEditWindow(int groupId)
        {
            InitializeComponent();
            _groupId = groupId;
            LoadStudents();
        }

        private void LoadStudents()
        {
            var students = _db.Student
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _groupId))
                .Include(s => s.Person)
                .ToList();

            foreach (var student in students)
            {
                _students.Add(new StudentAttendanceModel
                {
                    StudentId = student.StudentId,
                    StudentFullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}",
                    IsVisited = false
                });
            }

            StudentsList.ItemsSource = _students;
        }

        private void LoadData()
        {
            var classHour = _db.ClassHour
                .Include("StudentClassHour")
                .FirstOrDefault(ch => ch.ClassHourId == _classHourId);

            if (classHour == null) return;

            DateDatePicker.SelectedDate = classHour.Date;
            TopicTextBox.Text = classHour.Topic;
            DecisionTextBox.Text = classHour.Decision;

            var group = classHour.StudentClassHour.FirstOrDefault()?.Student?.StudyGroup.FirstOrDefault();
            if (group == null) return;

            _groupId = group.StudyGroupId;

            var students = _db.Student
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _groupId))
                .Include(s => s.Person)
                .ToList();

            foreach (var student in students)
            {
                var attendance = classHour.StudentClassHour.FirstOrDefault(sch => sch.StudentId == student.StudentId);
                _students.Add(new StudentAttendanceModel
                {
                    StudentId = student.StudentId,
                    StudentFullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}",
                    IsVisited = attendance?.IsVisited ?? false
                });
            }

            StudentsList.ItemsSource = _students;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!DateDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите дату");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TopicTextBox.Text))
                {
                    MessageBox.Show("Введите тему");
                    return;
                }

                var date = DateDatePicker.SelectedDate.Value;
                var topic = TopicTextBox.Text.Trim();
                var decision = DecisionTextBox.Text.Trim();

                if (_isEditMode)
                {
                    var dbClassHour = _db.ClassHour
                        .Include("StudentClassHour")
                        .FirstOrDefault(ch => ch.ClassHourId == _classHourId);

                    if (dbClassHour == null) return;

                    dbClassHour.Date = date;
                    dbClassHour.Topic = topic;
                    dbClassHour.Decision = decision;

                    // Удаляем старые данные
                    var existingRecords = dbClassHour.StudentClassHour.ToList();
                    _db.StudentClassHour.RemoveRange(existingRecords);

                    // Добавляем новые
                    foreach (var item in _students)
                    {
                        _db.StudentClassHour.Add(new StudentClassHour
                        {
                            ClassHourId = dbClassHour.ClassHourId,
                            StudentId = item.StudentId,
                            IsVisited = item.IsVisited
                        });
                    }

                    _db.SaveChanges();
                }
                else
                {
                    var newClassHour = new ClassHour
                    {
                        Date = date,
                        Topic = topic,
                        Decision = decision
                    };

                    _db.ClassHour.Add(newClassHour);
                    _db.SaveChanges();

                    foreach (var item in _students)
                    {
                        _db.StudentClassHour.Add(new StudentClassHour
                        {
                            ClassHourId = newClassHour.ClassHourId,
                            StudentId = item.StudentId,
                            IsVisited = item.IsVisited
                        });
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

    public class StudentAttendanceModel
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; }
        public bool IsVisited { get; set; }
    }
}