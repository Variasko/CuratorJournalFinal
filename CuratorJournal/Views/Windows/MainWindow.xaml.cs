using CuratorJournal.Session;
using CuratorJournal.Views.Pages;
using FontAwesome.WPF;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CuratorJournal.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Page _currentPage;
        public MainWindow()
        {
            InitializeComponent();
            _currentPage = new ProfilePage(this);
            NavigateTo();
            foreach (ImageAwesome i in ((SideMenuBorder.Child as ScrollViewer).Content as StackPanel).Children)
            {
                i.MouseDown += (s,e)=>
                {
                    NavigateTo();
                };
            }
            if (CurrentSession.CurrentCurator.User.IsAdmin)
                AdminOnly.Visibility = Visibility.Visible;
            else
                AdminOnly.Visibility = Visibility.Collapsed;
        }

        private void ApplicationShutDownButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void NavigateTo()
        {
            MainFrame.Navigate(_currentPage);
        }

        private void Profile_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentPage = new ProfilePage(this);
        }
        private void ActivistImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentPage = new ActivistPage();
        }

        private void SocialPassportImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentPage = new SocialPassportPage();
        }

        private void PatentMetengImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentPage = new ParentMeetingsPage();
        }

        private void DormitoryImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentPage = new DormitoryPage();
        }

        private void ClassHourImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentPage = new ClassHourPage();
        }

        private void HobbyImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentPage = new ExtracurricularPage();
        }

        private void CharacteriasticImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentPage = new CharacteristicPage();
        }

        private void IndividualWorkImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentPage = new IndividualWorkPage();
        }

        private void ReportImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentPage = new ReportPage();
        }

        private void ImageAwesome_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentPage = new MainReferencePage();
        }
    }
}
