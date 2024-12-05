using MyTodo.Core.Api;
using MyTodo.Core.Models;
using MyTodo.Modules.Login.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyTodo.Modules.Login.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        private readonly MyTodoClient _myTodoClient;
        private readonly IEventAggregator _eventAggregator;

        public string Title => "账号登录";

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

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

        public LoginViewModel(MyTodoClient myTodoClient, IEventAggregator eventAggregator)
        {
            _myTodoClient = myTodoClient;
            _eventAggregator = eventAggregator;
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

        /// <summary>
        /// 登录
        /// </summary>
        private async void ExecuteLoginCommand()
        {
            IsLoading = true;
            var response = await _myTodoClient.LoginAsync(LoginModel);
            IsLoading = false;

            // 登录成功
            if (response.IsSuccess)
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                return;
            }

            _eventAggregator.GetEvent<PopupMessageEvent>().Publish(response.Message);
        }

        private bool CanExecuteLoginCommand()
        {
            return Validator.TryValidateObject(LoginModel, new ValidationContext(LoginModel), null);
        }

        /// <summary>
        /// 注册
        /// </summary>
        private async void ExecuteRegisterCommand()
        {
            IsLoading = true;
            var response = await _myTodoClient.RegisterAsync(RegisterModel);
            IsLoading = false;

            if (response.IsSuccess)
            {
                _eventAggregator.GetEvent<PopupMessageEvent>().Publish("账号注册成功, 请登录");
                LoginModel.ClearModelWithoutValidate();
                ExecuteSwitchCommand();
                return;
            }

            _eventAggregator.GetEvent<PopupMessageEvent>().Publish(response.Message);
        }

        private bool CanExecuteRegisterCommand()
        {
            return Validator.TryValidateObject(
                RegisterModel,
                new ValidationContext(RegisterModel),
                null
            );
        }
    }
}
