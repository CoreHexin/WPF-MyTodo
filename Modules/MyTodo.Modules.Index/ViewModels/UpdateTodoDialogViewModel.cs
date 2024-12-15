using System;
using MyTodo.Core.Api;
using MyTodo.Core.DTOs;
using MyTodo.Core.Events;
using MyTodo.Core.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyTodo.Modules.Index.ViewModels
{
    public class UpdateTodoDialogViewModel : BindableBase, IDialogAware
    {
        private readonly ApiClient _apiClient;
        private readonly IEventAggregator _eventAggregator;

        public string Title => "编辑待办事项";

        public int Id { get; set; }

        public event Action<IDialogResult> RequestClose;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private TodoForUpdateDTO _todoForUpdateDTO = new TodoForUpdateDTO();
        public TodoForUpdateDTO TodoForUpdateDTO
        {
            get { return _todoForUpdateDTO; }
            set { SetProperty(ref _todoForUpdateDTO, value); }
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));

        private void ExecuteCancelCommand()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand));

        private async void ExecuteSaveCommand()
        {
            IsLoading = true;
            ApiResponse response = await _apiClient.UpdateTodoAsync(Id, TodoForUpdateDTO);
            IsLoading = false;

            if (!response.IsSuccess)
            {
                _eventAggregator.GetEvent<PopupMessageEvent>().Publish("更新失败, 请稍后重试");
                return;
            }

            _eventAggregator.GetEvent<PopupMessageEvent>().Publish("更新成功");
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        public UpdateTodoDialogViewModel(ApiClient apiClient, IEventAggregator eventAggregator)
        {
            _apiClient = apiClient;
            _eventAggregator = eventAggregator;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            TodoItem todoItem = parameters.GetValue<TodoItem>("TodoItem");

            Id = todoItem.Id;
            TodoForUpdateDTO.Title = todoItem.Title;
            TodoForUpdateDTO.Content = todoItem.Content;
            TodoForUpdateDTO.Status = todoItem.Status;
        }
    }
}
