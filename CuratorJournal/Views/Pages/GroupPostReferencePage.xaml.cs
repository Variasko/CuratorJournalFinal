using CuratorJournal.DataBase;
using CuratorJournal.Views.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Pages
{
    /// <summary>
    /// Interaction logic for GroupPostReferencePage.xaml
    /// </summary>
    public partial class GroupPostReferencePage : Page
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        public GroupPostReferencePage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MainDataGrid.ItemsSource = _db.GroupPost.ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            GroupPost selectedPost = (sender as Button).DataContext as GroupPost;

            new GroupPostAddEditWindow(selectedPost).ShowDialog();
            LoadData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            GroupPost selectedPost = (sender as Button).DataContext as GroupPost;
            
            MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите удалить должность {selectedPost.Name}?", 
                "Успех!", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;
            try
            {
                _db.GroupPost.Remove(selectedPost);
                _db.SaveChanges();
                MessageBox.Show("Должность успешно удалена", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            } catch
            {
                MessageBox.Show("Возникла проблема при удалении!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new GroupPostAddEditWindow().ShowDialog();
            LoadData();
        }
    }
}
