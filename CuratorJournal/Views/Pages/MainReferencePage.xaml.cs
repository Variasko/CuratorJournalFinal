using CuratorJournal.DataBase;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    public partial class MainReferencePage : Page
    {
        public MainReferencePage()
        {
            InitializeComponent();
        }

        private void OpenStudentPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StudentReferencePage());
        }

        private void OpenCuratorPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CuratorReferencePage());
        }

        private void OpenParentPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ParentReferencePage());
        }

        private void OpenGroupPostPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GroupPostReferencePage());
        }

        private void OpenHobbyPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HobbyReferencePage());
        }

        private void OpenQualificationPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new QualificationReferencePage());
        }

        private void OpenSpecificationPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SpecificationReferencePage());
        }

        private void OpenStudyGroupPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StudyGroupReferencePage());
        }

        private void OpenSocialStatusPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SocialStatusReferencePage());
        }

        private void OpenTeacherCategoryPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TeacherCategoryReferencePage());
        }

        private void OpenUserPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserReferencePage());
        }

        private void PromoteGroupsButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите перевести все группы на следующий курс?",
                                          "Подтверждение",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                return;

            using (var db = new CuratorJournalEntities())
            {
                try
                {
                    var groups = db.StudyGroup.ToList();

                    foreach (var group in groups)
                    {
                        // Пропускаем группы, которые уже были выпущены (Course = 0)
                        if (group.Course == 0) continue;

                        if (group.Course < 4)
                        {
                            group.Course += 1;
                        }
                        else if (group.Course == 4)
                        {
                            group.Course = 0; // Группа считается "выпущенной"
                        }
                    }

                    db.SaveChanges();
                    MessageBox.Show("Группы успешно переведены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    string errorMessage = $"Ошибка при переводе групп:\n{ex.Message}";
                    if (ex.InnerException != null)
                    {
                        errorMessage += $"\nInner Exception: {ex.InnerException.Message}";
                        errorMessage += $"\nA{ex.StackTrace}";
                        errorMessage += $"\nB{ex.Source}";
                        errorMessage += $"\nB{ex.TargetSite}";
                    }

                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}