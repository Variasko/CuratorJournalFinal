using CuratorJournal.DataBase;
using CuratorJournal.Helpers;
using CuratorJournal.Session;
using System.Data.Entity; // Для асинхронных методов EF6
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CuratorJournal.Views.Windows
{
    public partial class SignInWindow : Window
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private readonly MessageBoxHelper _messageBoxHelper = new MessageBoxHelper();

        public SignInWindow()
        {
            InitializeComponent();
            try
            {
                var test = _db.Curator.ToList();
            } catch (Exception ex)
            {
                _messageBoxHelper.ShowError("Ошибка подключения к серверу!\nОбратитесь к системному администратору!");
            }
            
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Блокируем кнопку на время выполнения
            SignInButton.IsEnabled = false;

            // Показываем анимацию загрузки
            ShowLoadingState();

            // Запускаем анимацию и авторизацию параллельно
            var animationTask = AnimateTextAsync(LoadingTextBox, "Авторизация...", 100);
            var authTask = AuthenticateUserAsync();

            // Ожидаем завершения обеих задач
            await Task.WhenAll(animationTask, authTask);
            var user = await authTask;

            // Обрабатываем результат авторизации
            HandleAuthResult(user);
        }

        private void ShowLoadingState()
        {
            LoadingBorder.Visibility = Visibility.Visible;
            AuthBorder.Visibility = Visibility.Hidden;
        }

        private void ShowAuthState()
        {
            LoadingBorder.Visibility = Visibility.Collapsed;
            AuthBorder.Visibility = Visibility.Visible;
            SignInButton.IsEnabled = true;
        }

        private async Task<User> AuthenticateUserAsync()
        {
            try
            {
                return await _db.User
                    .FirstOrDefaultAsync(u =>
                        u.Login == LoginTextBox.Text &&
                        u.Password == PasswordPasswordBox.Password);
            }
            catch
            {
                return null;
            }
        }

        private void HandleAuthResult(User user)
        {
            if (user == null)
            {
                ShowAuthState();
                _messageBoxHelper.ShowError("Неверные логин или пароль!");
                return;
            }

            LoadCuratorData(user);
            OpenMainWindow();
        }

        private void LoadCuratorData(User user)
        {
            CurrentSession.CurrentCurator = _db.Curator
                .AsNoTracking()
                .Include(c=>c.StudyGroup)
                .FirstOrDefault(c => c.UserId == user.UserId);
        }

        private void OpenMainWindow()
        {
            new MainWindow().Show();
            Close();
        }

        private async Task AnimateTextAsync(TextBlock textBlock, string fullText, int delayMs = 100)
        {
            textBlock.Text = string.Empty;
            var textBuilder = new StringBuilder();

            foreach (char c in fullText)
            {
                textBuilder.Append(c);
                textBlock.Text = textBuilder.ToString();
                await Task.Delay(delayMs);
            }
        }

        private void ApplicationShutDownButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
