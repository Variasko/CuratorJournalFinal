using CuratorJournal.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Windows
{
    public partial class IndividualWorkEditWindow : Window
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private bool _isEditMode = false;
        private int? _individualWorkId;
        private int _groupId;

        public bool IsSaved { get; private set; } = false;

        // Конструктор для редактирования
        public IndividualWorkEditWindow(int individualWorkId, int dummy)
        {
            InitializeComponent();
            _individualWorkId = individualWorkId;
            _isEditMode = true;
            LoadData();
        }

        // Конструктор для добавления
        public IndividualWorkEditWindow(int groupId)
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
            var work = _db.IndividualWork
                .Include("Student.Person")
                .Include("Parent")
                .FirstOrDefault(w => w.IndividualWorkId == _individualWorkId);

            if (work == null) return;

            TopicTextBox.Text = work.Topic;
            DecisionTextBox.Text = work.Decision;

            if (work.IsStudent && work.Student != null)
            {
                StudentRadio.IsChecked = true;
                var student = work.Student;
                var studentModel = new StudentModel
                {
                    StudentId = student.StudentId,
                    StudentFullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}"
                };

                StudentComboBox.ItemsSource = new List<StudentModel> { studentModel };
                StudentComboBox.SelectedValue = student.StudentId;

                ParentComboBox.Visibility = Visibility.Collapsed;
                StudentComboBox.Visibility = Visibility.Visible;
            }
            else if (!work.IsStudent && work.Parent != null)
            {
                ParentRadio.IsChecked = true;
                var parent = work.Parent;
                var parentModel = new ParentModel
                {
                    ParentId = parent.ParentId,
                    ParentFullName = $"{parent.Surname} {parent.Name} {parent.Patronymic}"
                };

                var parentsInGroup = _db.Parent
                    .Where(p => p.Student.Any(s => s.StudyGroup.Any(g => g.StudyGroupId == _groupId)))
                    .ToList();

                ParentComboBox.ItemsSource = parentsInGroup.Select(p => new ParentModel
                {
                    ParentId = p.ParentId,
                    ParentFullName = $"{p.Surname} {p.Name} {p.Patronymic}"
                }).ToList();

                ParentComboBox.SelectedValue = parentModel.ParentId;
                ParentComboBox.Visibility = Visibility.Visible;
                StudentComboBox.Visibility = Visibility.Collapsed;
            }

            TopicTextBox.Text = work.Topic;
            DecisionTextBox.Text = work.Decision;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TopicTextBox.Text))
                {
                    MessageBox.Show("Введите тему");
                    return;
                }

                IndividualWork work;

                if (_isEditMode)
                {
                    work = _db.IndividualWork.Find(_individualWorkId);
                    if (work == null)
                    {
                        MessageBox.Show("Запись не найдена");
                        return;
                    }
                }
                else
                {
                    work = new IndividualWork
                    {
                        Date = DateTime.Now
                    };
                }

                work.Topic = TopicTextBox.Text.Trim();
                work.Decision = DecisionTextBox.Text.Trim();
                work.IsStudent = StudentRadio.IsChecked == true;

                // Получаем ID куратора через студента или родителя
                int? curatorId = null;

                if (StudentRadio.IsChecked == true)
                {
                    if (StudentComboBox.SelectedItem == null)
                    {
                        MessageBox.Show("Выберите студента");
                        return;
                    }

                    var studentId = ((StudentModel)StudentComboBox.SelectedItem).StudentId;
                    var student = _db.Student.Find(studentId);

                    if (student?.StudyGroup.FirstOrDefault() is StudyGroup group)
                    {
                        curatorId = group.CuratorId;
                    }

                    work.StudentId = studentId;
                    work.ParentId = null;
                }
                else if (ParentRadio.IsChecked == true)
                {
                    if (ParentComboBox.SelectedItem == null)
                    {
                        MessageBox.Show("Выберите родителя");
                        return;
                    }

                    var parentId = ((ParentModel)ParentComboBox.SelectedItem).ParentId;
                    var parent = _db.Parent.Find(parentId);

                    var firstStudent = parent?.Student.FirstOrDefault();
                    if (firstStudent?.StudyGroup.FirstOrDefault() is StudyGroup group)
                    {
                        curatorId = group.CuratorId;
                    }

                    work.ParentId = parentId;
                    work.StudentId = null;
                }

                if (!_isEditMode)
                {
                    _db.IndividualWork.Add(work);
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

        private void PersonType_Checked(object sender, RoutedEventArgs e)
        {
            // Проверяем, были ли элементы загружены
            if (StudentComboBox == null || ParentComboBox == null)
                return;

            if (StudentRadio.IsChecked == true)
            {
                StudentComboBox.Visibility = Visibility.Visible;
                ParentComboBox.Visibility = Visibility.Collapsed;
            }
            else if (ParentRadio.IsChecked == true)
            {
                StudentComboBox.Visibility = Visibility.Collapsed;
                ParentComboBox.Visibility = Visibility.Visible;
            }
        }

        public class StudentModel
        {
            public int StudentId { get; set; }
            public string StudentFullName { get; set; }
        }

        public class ParentModel
        {
            public int ParentId { get; set; }
            public string ParentFullName { get; set; }
        }
    }
}