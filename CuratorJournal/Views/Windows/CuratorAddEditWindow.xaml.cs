using CuratorJournal.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    public partial class CuratorAddEditWindow : Window
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        private Curator _curator;
        private bool _isEditing;

        public List<TeacherCategory> TeacherCategories { get; set; }
        public List<User> Users { get; set; }

        public CuratorAddEditWindow(Curator curator)
        {
            InitializeComponent();
            _curator = curator;
            _isEditing = true;

            LoadDependencies();

            DataContext = this;

            if (_curator != null)
            {
                var person = _curator.Person;

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
                TeacherCategoryComboBox.SelectedValue = _curator.CategoryId;
                UserComboBox.SelectedValue = _curator.UserId;
            }
        }

        public CuratorAddEditWindow()
        {
            InitializeComponent();
            _isEditing = false;

            LoadDependencies();

            DataContext = this;

            WhenGetPassportPicker.SelectedDate = DateTime.Today;
        }

        private void LoadDependencies()
        {
            // Загружаем категории преподавателей
            TeacherCategories = _db.TeacherCategory.ToList();

            // Только пользователи без привязанных кураторов
            Users = _db.User.Where(u => !_db.Curator.Any(c => c.UserId == u.UserId)).ToList();
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
            int categoryId;
            int userId;

            if (!int.TryParse(TeacherCategoryComboBox.SelectedValue?.ToString(), out categoryId))
            {
                MessageBox.Show("Выберите категорию", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(UserComboBox.SelectedValue?.ToString(), out userId))
            {
                MessageBox.Show("Выберите пользователя", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Заполните обязательные поля: Фамилия и Имя", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_isEditing)
            {
                // Создание новой персоны
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

                // Создание нового куратора
                var newCurator = new Curator
                {
                    PersonId = person.PersonId,
                    CategoryId = categoryId,
                    UserId = userId
                };

                _db.Curator.Add(newCurator);
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Куратор успешно добавлен", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении:\n{ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                // Редактирование существующего куратора
                var dbCurator = _db.Curator.Find(_curator.CuratorId);
                var dbPerson = dbCurator.Person;

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

                dbCurator.CategoryId = categoryId;

                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Куратор успешно обновлён", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }

        // Разбирает паспорт на серию и номер
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
    }
}