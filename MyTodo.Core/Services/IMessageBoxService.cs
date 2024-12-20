using System.Windows;

namespace MyTodo.Core.Services
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string message, string caption);
    }
}
