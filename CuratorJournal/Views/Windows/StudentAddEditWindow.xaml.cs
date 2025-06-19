using CuratorJournal.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class StudentAddEditWindow : Window
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        private Student _student;
        private bool _isEditing;

        public List<StudyGroup> StudyGroups { get; set; }
        public List<ParentDto> Parents { get; set; }

        public StudentAddEditWindow(Student student)
        {
            InitializeComponent();
            _student = student;
            _isEditing = true;

            LoadDependencies();

            DataContext = this;

            var person = _student.Person;

            SurnameTextBox.Text = person.Surname;
            NameTextBox.Text = person.Name;
            PatronymicTextBox.Text = person.Patronymic;
            PassportTextBox.Text = $"{person.PassportSerial} {person.PassportNumber}";
            WhoGavePassportTextBox.Text = person.WhoGavePassport;
            WhenGetPassportPicker.SelectedDate = person.WhenGetPassport;
            INNTextBox.Text = person.INN;
            SNILSTextBox.Text = person.SNILS;
            PhoneTextBox.Text = person.Phone;
            EmailTextBox.Text = person.Email;

            // Группа
            var group = _student.StudyGroup.FirstOrDefault();
            if (group != null)
            {
                StudyGroupComboBox.SelectedValue = group.StudyGroupId;
            }

            // Родитель
            var parent = _student.Parent.FirstOrDefault();
            if (parent != null)
            {
                ParentComboBox.SelectedValue = parent.ParentId;
            }

            // Отчислен
            IsDeductionCheckBox.IsChecked = _student.IsDeduction;
            DateDeductionPicker.SelectedDate = _student.DateDeduction;
        }

        public StudentAddEditWindow()
        {
            InitializeComponent();
            _isEditing = false;

            LoadDependencies();

            DataContext = this;

            WhenGetPassportPicker.SelectedDate = DateTime.Today;
        }

        private void LoadDependencies()
        {
            StudyGroups = _db.StudyGroup.ToList();

            var Parents1 = _db.Parent.ToList();

            Parents = Parents1.Select(p => new ParentDto
            {
                ParentId = p.ParentId,
                FullName = $"{p.Surname} {p.Name} {p.Patronymic}"
            }).ToList();
                
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string surname = SurnameTextBox.Text.Trim();
            string name = NameTextBox.Text.Trim();
            string patronymic = PatronymicTextBox.Text.Trim();
            string passport = PassportTextBox.Text.Trim();
            string whoGavePassport = WhoGavePassportTextBox.Text.Trim();
            DateTime? whenGetPassport = WhenGetPassportPicker.SelectedDate;
            string inn = INNTextBox.Text.Trim();
            string snils = SNILSTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            int? studyGroupId = StudyGroupComboBox.SelectedValue as int?;
            int? parentId = ParentComboBox.SelectedValue as int?;
            bool isDeduction = IsDeductionCheckBox.IsChecked ?? false;
            DateTime? dateDeduction = DateDeductionPicker.SelectedDate;

            if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Заполните обязательные поля: Фамилия и Имя", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!studyGroupId.HasValue)
            {
                MessageBox.Show("Выберите учебную группу", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_isEditing)
            {
                var person = new Person
                {
                    Surname = surname,
                    Name = name,
                    Patronymic = patronymic,
                    Phone = phone,
                    Email = email,
                    WhoGavePassport = whoGavePassport,
                    WhenGetPassport = whenGetPassport ?? DateTime.Now,
                    INN = inn,
                    SNILS = snils
                };
                SplitAndSetPassport(person, passport);

                _db.Person.Add(person);
                _db.SaveChanges(); // Чтобы получить PersonId

                var newStudent = new Student
                {
                    StudentId = person.PersonId,
                    Person = person,
                    IsDeduction = isDeduction,
                    DateDeduction = isDeduction ? dateDeduction : null
                };

                newStudent.StudyGroup.Add(_db.StudyGroup.Find(studyGroupId.Value));

                if (parentId.HasValue)
                {
                    var parent = _db.Parent.Find(parentId.Value);
                    newStudent.Parent.Add(parent);
                }

                _db.Student.Add(newStudent);
                _db.SaveChanges();

                MessageBox.Show("Студент успешно добавлен", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                var dbStudent = _db.Student.Find(_student.StudentId);
                var dbPerson = dbStudent.Person;

                dbPerson.Surname = surname;
                dbPerson.Name = name;
                dbPerson.Patronymic = patronymic;
                dbPerson.Phone = phone;
                dbPerson.Email = email;
                dbPerson.WhoGavePassport = whoGavePassport;
                dbPerson.WhenGetPassport = whenGetPassport ?? DateTime.Now;
                dbPerson.INN = inn;
                dbPerson.SNILS = snils;
                SplitAndSetPassport(dbPerson, passport);

                dbStudent.IsDeduction = isDeduction;
                dbStudent.DateDeduction = isDeduction ? dateDeduction : null;

                // Обновляем группу
                var oldGroups = dbStudent.StudyGroup.ToList();
                foreach (var g in oldGroups)
                {
                    dbStudent.StudyGroup.Remove(g);
                }
                dbStudent.StudyGroup.Add(_db.StudyGroup.Find(studyGroupId.Value));

                // Обновляем родителя
                var oldParents = dbStudent.Parent.ToList();
                foreach (var p in oldParents)
                {
                    dbStudent.Parent.Remove(p);
                }

                if (parentId.HasValue)
                {
                    var parent = _db.Parent.Find(parentId.Value);
                    dbStudent.Parent.Add(parent);
                }

                _db.SaveChanges();
                MessageBox.Show("Студент успешно обновлён", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }

        private void SplitAndSetPassport(Person person, string passport)
        {
            if (string.IsNullOrWhiteSpace(passport))
            {
                person.PassportSerial = "";
                person.PassportNumber = "";
                return;
            }

            string[] parts = passport.Split(' ')
                                     .Where(p => !string.IsNullOrWhiteSpace(p))
                                     .ToArray();

            if (parts.Length >= 2)
            {
                person.PassportSerial = parts[0];
                person.PassportNumber = parts[1];
            }
            else
            {
                person.PassportSerial = passport.Substring(0, Math.Min(4, passport.Length));
                person.PassportNumber = passport.Length > 4 ? passport.Substring(4).Trim() : "";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void IsDeduction_Checked(object sender, RoutedEventArgs e)
        {
            DateDeductionPicker.IsEnabled = true;
        }

        private void IsDeduction_Unchecked(object sender, RoutedEventArgs e)
        {
            DateDeductionPicker.IsEnabled = false;
            DateDeductionPicker.SelectedDate = null;
        }
    }

    // DTO для отображения родителей
    public class ParentDto
    {
        public int ParentId { get; set; }
        public string FullName { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}