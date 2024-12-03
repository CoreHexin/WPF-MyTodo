using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyTodo.Modules.Login.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        public string Title => "账号登录";

        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { SetProperty(ref _selectedIndex, value); }
        }

        public string Password { private get; set; }

        public event Action<IDialogResult> RequestClose;

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(ExecuteLoginCommand));

        // 切换登录和注册页面
        private DelegateCommand _switchCommand;
        public DelegateCommand SwitchCommand =>
            _switchCommand ?? (_switchCommand = new DelegateCommand(ExecuteSwitchCommand));

        public LoginViewModel() { }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters) { }

        private void ExecuteSwitchCommand()
        {
            SelectedIndex = SelectedIndex == 0 ? 1 : 0;
        }

        void ExecuteLoginCommand()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }
    }
}
