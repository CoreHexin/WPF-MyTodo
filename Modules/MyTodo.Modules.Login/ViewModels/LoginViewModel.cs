using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyTodo.Modules.Login.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        public string Title => "账号登录";

        public event Action<IDialogResult> RequestClose;

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(ExecuteLoginCommand));

        void ExecuteLoginCommand()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        public LoginViewModel() { }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters) { }
    }
}
