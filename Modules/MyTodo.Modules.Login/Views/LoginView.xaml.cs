using System.Windows.Controls;
using MyTodo.Core.Events;
using Prism.Events;

namespace MyTodo.Modules.Login.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        private readonly IEventAggregator _eventAggregator;

        public LoginView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PopupMessageEvent>().Subscribe(PopupMessage);
        }

        private void PopupMessage(string msg)
        {
            snackbar.MessageQueue.Enqueue(msg);
        }
    }
}
