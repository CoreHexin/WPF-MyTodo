using System;
using System.ComponentModel.DataAnnotations;
using MyTodo.Core.Api;
using MyTodo.Core.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyTodo.Modules.Login.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        private readonly MyTodoClient _myTodoClient;

        public string Title => "账号登录";

        private LoginModel _loginModel = new LoginModel();
        public LoginModel LoginModel
        {
            get { return _loginModel; }
            set { SetProperty(ref _loginModel, value); }
        }

        private RegisterModel _registerModel = new RegisterModel();
        public RegisterModel RegisterModel
        {
            get { return _registerModel; }
            set { SetProperty(ref _registerModel, value); }
        }

        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { SetProperty(ref _selectedIndex, value); }
        }

        public event Action<IDialogResult> RequestClose;

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand =>
            _loginCommand
            ?? (
                _loginCommand = new DelegateCommand(ExecuteLoginCommand, CanExecuteLoginCommand)
                    .ObservesProperty(() => LoginModel.Email)
                    .ObservesProperty(() => LoginModel.Password)
            );

        private DelegateCommand _registerCommand;
        public DelegateCommand RegisterCommand =>
            _registerCommand
            ?? (
                _registerCommand = new DelegateCommand(
                    ExecuteRegisterCommand,
                    CanExecuteRegisterCommand
                )
                    .ObservesProperty(() => RegisterModel.Name)
                    .ObservesProperty(() => RegisterModel.Email)
                    .ObservesProperty(() => RegisterModel.Password)
                    .ObservesProperty(() => RegisterModel.ConfirmPassword)
            );

        // 切换登录和注册页面
        private DelegateCommand _switchCommand;

        public DelegateCommand SwitchCommand =>
            _switchCommand ?? (_switchCommand = new DelegateCommand(ExecuteSwitchCommand));

        public LoginViewModel(MyTodoClient myTodoClient)
        {
            _myTodoClient = myTodoClient;
        }

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

        private async void ExecuteLoginCommand()
        {
            var response = await _myTodoClient.LoginAsync(LoginModel);
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private bool CanExecuteLoginCommand()
        {
            return Validator.TryValidateObject(LoginModel, new ValidationContext(LoginModel), null);
        }

        private async void ExecuteRegisterCommand()
        {
            var response = await _myTodoClient.RegisterAsync(RegisterModel);

            // todo: 注册成功, 切换到登录

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private bool CanExecuteRegisterCommand()
        {
            return Validator.TryValidateObject(RegisterModel, new ValidationContext(RegisterModel), null);
        }
    }
}
