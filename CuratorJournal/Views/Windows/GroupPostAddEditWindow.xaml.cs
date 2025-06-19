using CuratorJournal.DataBase;
using System;
using System.Windows;

namespace CuratorJournal.Views.Windows
{
    /// <summary>
    /// Interaction logic for GroupPostAddEditWindow.xaml
    /// </summary>
    public partial class GroupPostAddEditWindow : Window
    {
        private CuratorJournalEntities _db = new CuratorJournalEntities();
        private GroupPost _groupPost;
        private bool _isEditing;
        public GroupPostAddEditWindow(GroupPost groupPost)
        {
            InitializeComponent();

            _groupPost = groupPost;
            _isEditing = true;
            GroupPostName.Text = groupPost.Name;
        }
        public GroupPostAddEditWindow()
        {
            InitializeComponent();

            _isEditing = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditing)
            {
                GroupPost groupPost = _db.GroupPost.Find(_groupPost.PostId);
                groupPost.Name = GroupPostName.Text;
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Должность успешно cохранена!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                } catch (Exception ex)
                {
                    MessageBox.Show($"Возникла проблема при добавлении!\n{ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                _db.GroupPost.Add(new GroupPost { Name = GroupPostName.Text });
                try
                {
                    _db.SaveChanges();
                    MessageBox.Show("Должность успешно cохранена!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch
                {
                    MessageBox.Show("Возникла проблема при добавлении!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
