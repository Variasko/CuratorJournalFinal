using CuratorJournal.DataBase;
using CuratorJournal.Models;
using System;
using System.Linq;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class DormitoryEditWindow : Window
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private readonly StudentDormitoryView _currentRecord;
        private readonly int _groupId;
        private readonly bool _isEditMode;

        public bool IsSaved { get; private set; } = false;

        public DormitoryEditWindow(int groupId)
        {
            InitializeComponent();
            _groupId = groupId;
            _isEditMode = false;
            InitializeForAddMode();
        }

        public DormitoryEditWindow(StudentDormitoryView record)
        {
            InitializeComponent();
            _currentRecord = record;
            _isEditMode = true;
            InitializeForEditMode();
        }

        private void InitializeForAddMode()
        {
            StudentComboBox.Visibility = Visibility.Visible;
            StudentTextBox.Visibility = Visibility.Collapsed;
            LoadStudents();
        }

        private void InitializeForEditMode()
        {
            StudentTextBox.Visibility = Visibility.Visible;
            StudentComboBox.Visibility = Visibility.Collapsed;
            LoadData();
        }

        private void LoadStudents()
        {
            try
            {
                // Загружаем студентов без информации об общежитии
                var students = _db.Student
                    .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _groupId) &&
                                s.StudentInDormitory == null) // Изменено с !s.StudentInDormitory.Any()
                    .ToList()
                    .Select(s => new StudentDormitoryView
                    {
                        StudentId = s.StudentId,
                        StudentFullName = $"{s.Person.Surname} {s.Person.Name} {s.Person.Patronymic}"
                    })
                    .ToList();

                StudentComboBox.ItemsSource = students;
                StudentComboBox.DisplayMemberPath = "StudentFullName";
                StudentComboBox.SelectedValuePath = "StudentId";

                if (students.Any())
                {
                    StudentComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки студентов: {ex.Message}");
            }
        }

        private void LoadData()
        {
            if (_currentRecord != null)
            {
                StudentTextBox.Text = _currentRecord.StudentFullName;
                RoomTextBox.Text = _currentRecord.Room.ToString();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (string.IsNullOrWhiteSpace(RoomTextBox.Text))
                {
                    MessageBox.Show("Введите номер комнаты");
                    return;
                }

                if (!int.TryParse(RoomTextBox.Text, out int room) || room <= 0)
                {
                    MessageBox.Show("Номер комнаты должен быть положительным числом");
                    return;
                }

                if (_isEditMode)
                {
                    // Редактирование существующей записи
                    var student = _db.Student
                        .Include("StudentInDormitory")
                        .FirstOrDefault(s => s.StudentId == _currentRecord.StudentId);

                    if (student != null && student.StudentInDormitory != null)
                    {
                        student.StudentInDormitory.Room = room;
                        _db.SaveChanges();
                        IsSaved = true;
                        Close();
                    }
                }
                else
                {
                    // Добавление новой записи
                    if (StudentComboBox.SelectedItem == null)
                    {
                        MessageBox.Show("Выберите студента");
                        return;
                    }

                    var selectedStudent = (StudentDormitoryView)StudentComboBox.SelectedItem;
                    var student = _db.Student
                        .FirstOrDefault(s => s.StudentId == selectedStudent.StudentId);

                    if (student == null)
                    {
                        MessageBox.Show("Студент не найден");
                        return;
                    }

                    // Проверяем, что у студента еще нет записи
                    if (student.StudentInDormitory != null)
                    {
                        MessageBox.Show("У этого студента уже есть информация о проживании");
                        return;
                    }

                    // Создаем новую запись
                    var dormitory = new StudentInDormitory
                    {
                        StudentId = student.StudentId,
                        Room = room
                    };

                    _db.StudentInDormitory.Add(dormitory);
                    _db.SaveChanges();
                    IsSaved = true;
                    Close();
                }
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