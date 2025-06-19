using System.Windows;

namespace CuratorJournal.Helpers
{
    internal class MessageBoxHelper
    {
        public void ShowError(string mess)
        {
            MessageBox.Show(
                    mess,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
        }
        public void ShowInfo(string mess)
        {
            MessageBox.Show(
                    mess,
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
        }
        public MessageBoxResult ShowAsk(string mess, string title)
        {
            return MessageBox.Show(
                    mess,
                    title,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Asterisk
                );
        }
    }
}
