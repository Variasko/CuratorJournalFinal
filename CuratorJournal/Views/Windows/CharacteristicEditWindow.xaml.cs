using CuratorJournal.DataBase;
using CuratorJournal.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Windows
{
    public partial class CharacteristicEditWindow : Window
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private bool _isEditMode = false;
        private int? _studentId;
        private int _groupId;

        public bool IsSaved { get; private set; } = false;

        // Конструктор для редактирования
        public CharacteristicEditWindow(int studentId, int dummy)
        {
            InitializeComponent();
            _studentId = studentId;
            _isEditMode = true;
            LoadData();
        }

        // Конструктор для добавления
        public CharacteristicEditWindow(int groupId)
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

            StudentComboBox.ItemsSource = students.Select(s => new StudentModel
            {
                StudentId = s.StudentId,
                StudentFullName = $"{s.Person.Surname} {s.Person.Name} {s.Person.Patronymic}"
            }).ToList();
        }

        private void LoadData()
        {
            var student = _db.Student
                .Include("Person")
                .Include("CuratorCharacteristic")
                .FirstOrDefault(s => s.StudentId == _studentId);

            if (student == null) return;

            var model = new StudentModel
            {
                StudentId = student.StudentId,
                StudentFullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}"
            };

            StudentComboBox.ItemsSource = new List<StudentModel> { model };
            StudentComboBox.SelectedValue = model.StudentId;

            var lastChar = student.CuratorCharacteristic
                .OrderByDescending(c => c.DateCreated)
                .FirstOrDefault();

            if (lastChar != null)
            {
                CharacteristicTextBox.Text = lastChar.Characteristic;
            }
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

                var selectedStudent = (StudentModel)StudentComboBox.SelectedItem;

                var student = _db.Student
                    .Include("CuratorCharacteristic")
                    .FirstOrDefault(s => s.StudentId == selectedStudent.StudentId);

                if (student == null)
                {
                    MessageBox.Show("Студент не найден");
                    return;
                }

                string text = CharacteristicTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(text))
                {
                    MessageBox.Show("Введите текст характеристики");
                    return;
                }

                if (_isEditMode)
                {
                    // Редактирование — обновляем последнюю запись
                    var charToUpdate = student.CuratorCharacteristic
                        .OrderByDescending(c => c.DateCreated)
                        .FirstOrDefault();

                    if (charToUpdate != null)
                    {
                        charToUpdate.Characteristic = text;
                        charToUpdate.DateCreated = DateTime.Now;
                    }
                }
                else
                {
                    // Добавление новой характеристики
                    var newChar = new CuratorCharacteristic
                    {
                        StudentId = student.StudentId,
                        CuratorId = CurrentSession.CurrentCurator.CuratorId,
                        Characteristic = text,
                        DateCreated = DateTime.Now
                    };

                    _db.CuratorCharacteristic.Add(newChar);
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

        // Модель для отображения в ComboBox
        public class StudentModel
        {
            public int StudentId { get; set; }
            public string StudentFullName { get; set; }
        }
    }
}