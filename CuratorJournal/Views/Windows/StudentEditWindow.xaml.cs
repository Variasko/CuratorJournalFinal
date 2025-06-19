using CuratorJournal.DataBase;
using System;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class StudentEditWindow : Window
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private int? _studentId;
        public bool IsSaved { get; private set; }

        public StudentEditWindow()
        {
            InitializeComponent();
            _studentId = null;
        }

        public StudentEditWindow(int studentId) : this()
        {
            _studentId = studentId;
            LoadData();
        }

        private void LoadData()
        {
            var student = _db.Student.Find(_studentId);
            if (student == null || student.Person == null) return;

            SurnameTextBox.Text = student.Person.Surname;
            NameTextBox.Text = student.Person.Name;
            PatronymicTextBox.Text = student.Person.Patronymic;
            PassportSerialTextBox.Text = student.Person.PassportSerial;
            PassportNumberTextBox.Text = student.Person.PassportNumber;
            INNTextBox.Text = student.Person.INN;
            DateDeductionPicker.SelectedDate = student.DateDeduction;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_studentId.HasValue)
                {
                    // Редактирование
                    var student = _db.Student.Find(_studentId.Value);
                    var person = student.Person;

                    person.Surname = SurnameTextBox.Text;
                    person.Name = NameTextBox.Text;
                    person.Patronymic = PatronymicTextBox.Text;
                    person.PassportSerial = PassportSerialTextBox.Text;
                    person.PassportNumber = PassportNumberTextBox.Text;
                    person.INN = INNTextBox.Text;

                    student.DateDeduction = DateDeductionPicker.SelectedDate;

                    _db.SaveChanges();
                }
                else
                {
                    // Добавление
                    var person = new Person
                    {
                        Surname = SurnameTextBox.Text,
                        Name = NameTextBox.Text,
                        Patronymic = PatronymicTextBox.Text,
                        PassportSerial = PassportSerialTextBox.Text,
                        PassportNumber = PassportNumberTextBox.Text,
                        INN = INNTextBox.Text
                    };

                    _db.Person.Add(person);
                    _db.SaveChanges();

                    var student = new Student
                    {
                        Person = person,
                        DateDeduction = DateDeductionPicker.SelectedDate
                    };

                    _db.Student.Add(student);
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
    }
}