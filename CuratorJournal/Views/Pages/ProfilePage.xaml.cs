using CuratorJournal.DataBase;
using CuratorJournal.Helpers;
using CuratorJournal.Session;
using CuratorJournal.Views.Windows;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CuratorJournal.Views.Pages
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();

        private Window _window;
        private Curator _curator;
        public ProfilePage(Window window)
        {
            InitializeComponent();
            _window = window;
            if (CurrentSession.CurrentCurator == null)
                return;
            _curator = CurrentSession.CurrentCurator;
            SurnameTextBox.Text = _curator.Person.Surname;
            NameTextBox.Text = _curator.Person.Name;
            PatronymicTextBox.Text = _curator.Person.Patronymic;
            LoadAvatar();
        }

        private void ImageAwesome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var result = new MessageBoxHelper().ShowAsk("Вы уверены что хотите выйти?", "Предупреждение");
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                new SignInWindow().Show();
                _window.Close();
            }
        }

        private void LoadAvatar()
        {
            var person = _db.Person.Find(_curator.Person.PersonId);
            if (person.Photo != null && person.Photo.Length > 0)
            {
                using (var ms = new MemoryStream(person.Photo))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ProfileImage.Source = bitmap;
                }
            }
        }

        private void ImageAwesome_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                byte[] photoBytes = File.ReadAllBytes(filePath);

                var person = _db.Person.Where(p => p.PersonId == _curator.Person.PersonId).First();
                if (person != null)
                {
                    person.Photo = photoBytes;
                    try
                    {
                        _db.SaveChanges();
                        new MessageBoxHelper().ShowInfo("Фото успешно загружено!");
                    } catch
                    {
                        new MessageBoxHelper().ShowError("Возникла ошибка при загрузке фото!");
                    }

                }
            }
            LoadAvatar();
        }
    }
}
