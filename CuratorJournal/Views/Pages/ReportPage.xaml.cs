using CuratorJournal.DataBase;
using CuratorJournal.Reports;
using CuratorJournal.Session;
using Microsoft.Win32;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class ReportPage : Page
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private StudyGroup _currentGroup;

        public ReportPage()
        {
            InitializeComponent();
            LoadGroups();
        }

        private void LoadGroups()
        {
            GroupsComboBox.ItemsSource = _db.StudyGroup
                .Where(g => g.CuratorId == CurrentSession.CurrentCurator.CuratorId)
                .ToList();

            GroupsComboBox.DisplayMemberPath = "FullName";
            GroupsComboBox.SelectedValuePath = "StudyGroupId";

            if (GroupsComboBox.Items.Count > 0)
                GroupsComboBox.SelectedIndex = 0;
        }


        private void GroupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupsComboBox.SelectedItem is StudyGroup selectedGroup)
            {
                _currentGroup = selectedGroup;
            }
            else
            {
                _currentGroup = null;
            }
        }

        private void GenerateSocialPassport_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var students = _db.Student.ToList();
            var socialStatuses = _db.SocialStatus.ToList();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"Социальный паспорт группы № {_currentGroup.FullName}";
            saveFileDialog.DefaultExt = ".docx";
            saveFileDialog.Filter = "Документ (.docx) | *.docx";
            if (saveFileDialog.ShowDialog() == true)
            {
                string file = saveFileDialog.FileName;
                var generator = new SocialPassportReportGenerator();
                generator.GenerateReport(file, _currentGroup.FullName, students, socialStatuses);
                MessageBox.Show(
                        "Отчёт успешно создан",
                        "Успех!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
            }
        }
        private void GenerateDormitory_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем список студентов группы с проживанием в общежитии
            var students = _db.Student
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId))
                .Include(s => s.Person)
                .Include(s => s.StudentInDormitory)
                .ToList();

            int currentCourse = _currentGroup.Course;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"Список студентов проживающих в общежитии - {_currentGroup.FullName}";
            saveFileDialog.DefaultExt = ".docx";
            saveFileDialog.Filter = "Документ (.docx) | *.docx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                var generator = new DormitoryReportGenerator();
                generator.GenerateReport(filePath, _currentGroup.FullName, students, currentCourse);

                MessageBox.Show(
                    "Отчёт успешно создан",
                    "Успех!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
        private void GenerateActivistReport_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Загружаем студентов с необходимыми связями
            var students = _db.Student
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId))
                .Include(s => s.Person)
                .Include(s => s.GroupPost)
                .ToList();

            int currentCourse = _currentGroup.Course;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"Актив группы № {_currentGroup.FullName}";
            saveFileDialog.DefaultExt = ".docx";
            saveFileDialog.Filter = "Документ (.docx) | *.docx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                var generator = new ActivistReportGenerator();
                generator.GenerateReport(filePath, _currentGroup.FullName, students, currentCourse);

                MessageBox.Show(
                    "Отчёт успешно создан",
                    "Успех!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
        private void GenerateExtracurricularActivityReport_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Загружаем студентов с необходимыми связями
            var students = _db.Student
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId))
                .Include(s => s.Person)
                .Include(s => s.Hobby) // Подключаем хобби
                .ToList();

            int currentCourse = _currentGroup.Course;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"Внеучебная занятость группы № {_currentGroup.FullName}";
            saveFileDialog.DefaultExt = ".docx";
            saveFileDialog.Filter = "Документ (.docx) | *.docx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                var generator = new ExtracurricularActivityReportGenerator();
                generator.GenerateReport(filePath, _currentGroup.FullName, students, currentCourse);

                MessageBox.Show(
                    "Отчёт успешно создан",
                    "Успех!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
        private void GenerateClassAttendanceReport_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Загружаем студентов группы с необходимыми связями
            var students = _db.Student
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId))
                .Include(s => s.Person)
                .Include(s => s.StudentClassHour)
                .ToList();

            // Получаем все классные часы группы
            var classHours = _db.ClassHour
                .Where(ch => ch.StudentClassHour.Any(sc => sc.Student.StudyGroup
                    .Any(g => g.StudyGroupId == _currentGroup.StudyGroupId)))
                .OrderBy(ch => ch.Date)
                .ToList();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"Учет посещений классных часов - {_currentGroup.FullName}";
            saveFileDialog.DefaultExt = ".docx";
            saveFileDialog.Filter = "Документ (.docx) | *.docx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                var generator = new ClassAttendanceReportGenerator();
                generator.GenerateReport(filePath, _currentGroup.FullName, students, classHours);

                MessageBox.Show(
                    "Отчёт успешно создан",
                    "Успех!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
        private void GenerateCuratorObservationList_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем список студентов группы с характеристиками
            var students = _db.Student
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == _currentGroup.StudyGroupId))
                .Include(s => s.Person)
                .Include(s => s.CuratorCharacteristic)
                .ToList();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"Лист наблюдений куратора - {_currentGroup.FullName}";
            saveFileDialog.DefaultExt = ".docx";
            saveFileDialog.Filter = "Документ (.docx) | *.docx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                var generator = new CuratorObservationListGenerator();
                generator.GenerateReport(filePath, _currentGroup.FullName, students);

                MessageBox.Show(
                    "Отчёт успешно создан",
                    "Успех!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
        private void GenerateParentMeetingProtocol_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"Протоколы родительских собраний - {_currentGroup.FullName}";
            saveFileDialog.DefaultExt = ".docx";
            saveFileDialog.Filter = "Документ Word (.docx) | *.docx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                var generator = new ParentMeetingProtocolGenerator(_db);
                generator.GenerateReport(filePath, _currentGroup);

                MessageBox.Show(
                    "Протоколы успешно созданы",
                    "Успех!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
        private void GenerateStudentReport_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"Отчёт по индивидуальной работе со студентами - {_currentGroup.FullName}";
            saveFileDialog.DefaultExt = ".docx";
            saveFileDialog.Filter = "Документ Word (.docx)|*.docx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                var generator = new IndividualWorkReportGenerator(_db);
                generator.GenerateStudentReport(filePath, _currentGroup);

                MessageBox.Show("Отчёт успешно создан", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void GenerateParentReport_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("Выберите группу из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"Отчёт по индивидуальной работе с родителями - {_currentGroup.FullName}";
            saveFileDialog.DefaultExt = ".docx";
            saveFileDialog.Filter = "Документ Word (.docx)|*.docx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                var generator = new IndividualWorkReportGenerator(_db);
                generator.GenerateParentReport(filePath, _currentGroup);

                MessageBox.Show("Отчёт успешно создан", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }   
    }
}