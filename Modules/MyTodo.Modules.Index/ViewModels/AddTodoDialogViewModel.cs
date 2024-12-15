using System;
using System.ComponentModel.DataAnnotations;
using MyTodo.Core.Api;
using MyTodo.Core.DTOs;
using MyTodo.Core.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyTodo.Modules.Index.ViewModels
{
    public class AddTodoDialogViewModel : BindableBase, IDialogAware
    {
        private readonly ApiClient _apiClient;
        private readonly IEventAggregator _eventAggregator;

        public string Title => "添加待办事项";

        public event Action<IDialogResult> RequestClose;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private TodoForCreateDTO _todoForCreateDTO = new TodoForCreateDTO();
        public TodoForCreateDTO TodoForCreateDTO
        {
            get { return _todoForCreateDTO; }
            set { SetProperty(ref _todoForCreateDTO, value); }
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));

        private DelegateCommand _saveCommand;

        public DelegateCommand SaveCommand =>
            _saveCommand
            ?? (
                _saveCommand = new DelegateCommand(ExecuteSaveCommand, CanExecuteSaveCommand)
                    .ObservesProperty(() => TodoForCreateDTO.Title)
                    .ObservesProperty(() => TodoForCreateDTO.Content)
            );

        public AddTodoDialogViewModel(ApiClient apiClient, IEventAggregator eventAggregator)
        {
            _apiClient = apiClient;
            _eventAggregator = eventAggregator;
        }

        private bool CanExecuteSaveCommand()
        {
            return Validator.TryValidateObject(
                TodoForCreateDTO,
                new ValidationContext(TodoForCreateDTO),
                null
            );
        }

        /// <summary>
        /// 通过API保存待办事项数据
        /// </summary>
        private async void ExecuteSaveCommand()
        {
            IsLoading = true;
            var response = await _apiClient.CreateTodoAsync(TodoForCreateDTO);
            IsLoading = false;

            if (!response.IsSuccess)
            {
                _eventAggregator
                    .GetEvent<PopupMessageEvent>()
                    .Publish("创建待办事项失败, 请稍后重试");
                return;
            }

            _eventAggregator.GetEvent<PopupMessageEvent>().Publish("创建待办事项成功");
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void ExecuteCancelCommand()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters) { }
    }
}
