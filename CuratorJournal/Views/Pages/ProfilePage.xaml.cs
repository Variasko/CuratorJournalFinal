using CuratorJournal.DataBase;
using CuratorJournal.Helpers;
using CuratorJournal.Session;
using CuratorJournal.Views.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CuratorJournal.Views.Pages
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
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
    }
}
