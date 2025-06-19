using CuratorJournal.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class StudyGroupAddEditWindow : Window
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        private StudyGroup _group;
        private bool _isEditing;

        public List<Specification> Specifications { get; set; }
        public List<Qualification> Qualifications { get; set; }
        public List<CuratorDto> CuratorsDto { get; set; }

        public StudyGroupAddEditWindow(StudyGroup group)
        {
            InitializeComponent();
            _group = group;
            _isEditing = true;

            Specifications = _db.Specification.ToList();
            Qualifications = _db.Qualification.ToList();
            var Curators = _db.Curator
                .Include("Person")
                .ToList();

            CuratorsDto = Curators.Select(c => new CuratorDto
            {
                CuratorId = c.CuratorId,
                PersonId = c.PersonId,
                Curator = c,
                Person = c.Person,
                FullName = $"{c.Person.Surname} {c.Person.Name} {c.Person.Patronymic}"
            }).ToList();

            DataContext = this;

            SpecificationComboBox.SelectedValue = group.SpecificationAbbreviation;
            QualificationComboBox.SelectedValue = group.QualificationAbbreviation;
            DateCreatePicker.SelectedDate = group.DateCreate;
            CourseTextBox.Text = group.Course.ToString();
            CuratorComboBox.SelectedValue = group.CuratorId;
            IsBudgetCheckBox.IsChecked = group.IsBuget;
        }

        public StudyGroupAddEditWindow()
        {
            InitializeComponent();
            _isEditing = false;

            Specifications = _db.Specification.ToList();
            Qualifications = _db.Qualification.ToList();
            var Curators = _db.Curator
                .Include("Person")
                .ToList();

            CuratorsDto = Curators.Select(c => new CuratorDto
            {
                CuratorId = c.CuratorId,
                PersonId = c.PersonId,
                Curator = c,
                Person = c.Person,
                FullName = $"{c.Person.Surname} {c.Person.Name} {c.Person.Patronymic}"
            }).ToList();

            DataContext = this;

            DateCreatePicker.SelectedDate = DateTime.Today;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string specAbbr = SpecificationComboBox.SelectedValue?.ToString();
            string qualAbbr = QualificationComboBox.SelectedValue?.ToString();
            DateTime dateCreate = DateCreatePicker.SelectedDate ?? DateTime.Now;
            int course;
            int curatorId;
            bool isBudget = IsBudgetCheckBox.IsChecked ?? false;

            if (string.IsNullOrWhiteSpace(specAbbr) || string.IsNullOrWhiteSpace(qualAbbr))
            {
                MessageBox.Show("Выберите специальность и квалификацию", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(CourseTextBox.Text, out course) || course < 1 || course > 4)
            {
                MessageBox.Show("Введите корректный номер курса (от 1 до 4)", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(CuratorComboBox.SelectedValue?.ToString(), out curatorId))
            {
                MessageBox.Show("Выберите куратора", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditing)
            {
                StudyGroup group = _db.StudyGroup.Find(_group.StudyGroupId);
                group.SpecificationAbbreviation = specAbbr;
                group.QualificationAbbreviation = qualAbbr;
                group.DateCreate = dateCreate;
                group.Course = course;
                group.CuratorId = curatorId;
                group.IsBuget = isBudget;

                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Группа успешно обновлена", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения:\n{ex.Message}\n\nInner Exception: {ex.InnerException?.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                _db.StudyGroup.Add(new StudyGroup
                {
                    SpecificationAbbreviation = specAbbr,
                    QualificationAbbreviation = qualAbbr,
                    DateCreate = dateCreate,
                    Course = course,
                    CuratorId = curatorId,
                    IsBuget = isBudget
                });

                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Группа успешно добавлена", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка добавления:\n{ex.Message}\n\nInner Exception: {ex.InnerException?.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    public class CuratorDto
    {
        public int CuratorId { get; set; }
        public int PersonId { get; set; }
        public Curator Curator { get; set; }
        public Person Person { get; set; }
        public string FullName { get; set; }
    }
}