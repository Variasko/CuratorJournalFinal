using CuratorJournal.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Windows
{
    public partial class ActivistEditWindow : Window
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private bool _isEditMode = false;
        private int? _studentId;
        private int _groupId;
        private string _currentPost;

        public bool IsSaved { get; private set; } = false;

        // Конструктор для добавления
        public ActivistEditWindow(int groupId)
        {
            InitializeComponent();
            _groupId = groupId;
            LoadForAddMode();
        }

        // Конструктор для редактирования
        public ActivistEditWindow(int studentId, string currentPost)
        {
            InitializeComponent();
            _studentId = studentId;
            _currentPost = currentPost;
            _isEditMode = true;
            LoadForEditMode(studentId, currentPost);
        }

        private void LoadForAddMode()
        {
            // Все студенты группы
            var studentsInGroup = _db.Student
                .Include("Person")
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _groupId))
                .ToList();

            StudentComboBox.ItemsSource = studentsInGroup.Select(s => new StudentModel
            {
                StudentId = s.StudentId,
                StudentFullName = $"{s.Person.Surname} {s.Person.Name} {s.Person.Patronymic}"
            }).ToList();

            if (StudentComboBox.Items.Count > 0)
                StudentComboBox.SelectedIndex = 0;

            // Все доступные должности (те, кто ещё никем не заняты в этой группе)
            var busyPosts = _db.Student
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _groupId) &&
                            s.GroupPost.Any())
                .SelectMany(s => s.GroupPost)
                .Select(p => p.PostId)
                .ToList();

            var availablePosts = _db.GroupPost
                .Where(p => !busyPosts.Contains(p.PostId))
                .ToList();

            PostComboBox.ItemsSource = availablePosts;
        }

        private void LoadForEditMode(int studentId, string currentPost)
        {
            var student = _db.Student
                .Include("Person")
                .Include("GroupPost")
                .FirstOrDefault(s => s.StudentId == studentId);

            if (student == null) return;

            // Отображаем студента
            var model = new StudentModel
            {
                StudentId = student.StudentId,
                StudentFullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}"
            };

            StudentComboBox.Visibility = Visibility.Collapsed;
            StudentComboBox.IsEnabled = false;

            // Загружаем все должности
            var allPosts = _db.GroupPost.ToList();
            PostComboBox.ItemsSource = allPosts;

            // Выбираем текущую
            var current = allPosts.FirstOrDefault(p => p.Name == currentPost);
            if (current != null)
            {
                PostComboBox.SelectedItem = current;
            }

            // Исключаем занятые должности в группе этого студента
            var group = student.StudyGroup.FirstOrDefault();
            if (group != null)
            {
                var otherStudents = _db.Student
                    .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == group.StudyGroupId) &&
                                s.StudentId != studentId)
                    .SelectMany(s => s.GroupPost)
                    .Select(p => p.PostId)
                    .ToList();

                foreach (var post in allPosts)
                {
                    if (otherStudents.Contains(post.PostId))
                    {
                        PostComboBox.Items.Remove(post);
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StudentComboBox.SelectedItem == null || PostComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите студента и должность");
                    return;
                }

                var selectedStudent = (StudentModel)StudentComboBox.SelectedItem;
                var selectedPost = (GroupPost)PostComboBox.SelectedItem;

                var student = _db.Student
                    .Include("GroupPost")
                    .FirstOrDefault(s => s.StudentId == selectedStudent.StudentId);

                if (student == null)
                {
                    MessageBox.Show("Студент не найден");
                    return;
                }

                // Проверяем, занята ли эта должность другим студентом в той же группе
                var group = student.StudyGroup.FirstOrDefault();
                if (group == null)
                {
                    MessageBox.Show("Не удалось определить группу студента");
                    return;
                }

                var isOccupied = _db.Student
                    .Any(s => s.StudentId != student.StudentId &&
                              s.GroupPost.Any(p => p.PostId == selectedPost.PostId) &&
                              s.StudyGroup.Any(g => g.StudyGroupId == group.StudyGroupId));

                if (isOccupied)
                {
                    MessageBox.Show("Эта должность уже занята другим студентом.");
                    return;
                }

                if (_isEditMode)
                {
                    // Удаляем старые должности
                    foreach (var oldPost in student.GroupPost.ToList())
                    {
                        student.GroupPost.Remove(oldPost);
                    }
                }
                else
                {
                    // Проверяем, нет ли уже назначенной должности
                    if (student.GroupPost.Any())
                    {
                        MessageBox.Show("У этого студента уже есть должность");
                        return;
                    }
                }

                // Назначаем новую
                student.GroupPost.Add(selectedPost);
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

        public class StudentModel
        {
            public int StudentId { get; set; }
            public string StudentFullName { get; set; }
        }
    }
}