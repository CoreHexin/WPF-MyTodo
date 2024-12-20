using System.Windows;

namespace MyTodo.Core.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public MessageBoxResult Show(string message, string caption)
        {
            return MessageBox.Show(message, caption, MessageBoxButton.OKCancel);
        }
    }
}
