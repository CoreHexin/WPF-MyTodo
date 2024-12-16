using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using MyTodo.Core.Api;
using MyTodo.Core.DTOs;
using MyTodo.Core.Events;
using MyTodo.Core.Helpers;
using MyTodo.Core.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace MyTodo.Modules.Todo.ViewModels
{
    public class TodoViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ApiClient _apiClient;

        public List<TodoStatus> TodoStatuses { get; set; }

        public TodoStatus SearchStatus { get; set; }

        private string _searchTitle;

        public string SearchTitle 
        {
            get
            {
                return _searchTitle;
            }
            set
            {
                if (_searchTitle != value)
                {
                    _searchTitle = value;
                    ExecuteSearchCommand();
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private bool _isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return _isRightDrawerOpen; }
            set { SetProperty(ref _isRightDrawerOpen, value); }
        }

        private TodoForCreateDTO _todoForCreateDTO = new TodoForCreateDTO();
        public TodoForCreateDTO TodoForCreateDTO
        {
            get { return _todoForCreateDTO; }
            set { SetProperty(ref _todoForCreateDTO, value); }
        }

        private ObservableCollection<TodoItem> _todoItems;
        public ObservableCollection<TodoItem> TodoItems
        {
            get { return _todoItems; }
            set { SetProperty(ref _todoItems, value); }
        }

        private DelegateCommand _openRightDrawerCommand;

        public DelegateCommand OpenRightDrawerCommand =>
            _openRightDrawerCommand
            ?? (_openRightDrawerCommand = new DelegateCommand(ExecuteOpenRightDrawerCommand));

        private void ExecuteOpenRightDrawerCommand()
        {
            IsRightDrawerOpen = true;
        }

        private DelegateCommand _loadDataCommand;
        public DelegateCommand LoadDataCommand =>
            _loadDataCommand ?? (_loadDataCommand = new DelegateCommand(ExecuteLoadDataCommand));

        private async void ExecuteLoadDataCommand()
        {
            IsLoading = true;
            await RefreshTodoItemsAsync();
            IsLoading = false;
        }

        private DelegateCommand _saveCommand;

        public DelegateCommand SaveCommand =>
            _saveCommand
            ?? (
                _saveCommand = new DelegateCommand(ExecuteSaveCommand, CanExecuteSaveCommand)
                    .ObservesProperty(() => TodoForCreateDTO.Title)
                    .ObservesProperty(() => TodoForCreateDTO.Content)
            );

        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand =>
            _searchCommand ?? (_searchCommand = new DelegateCommand(ExecuteSearchCommand));

        private async void ExecuteSearchCommand()
        {
            IsLoading = true;
            TodoQueryObject todoQueryObject = new TodoQueryObject()
            {
                Title = SearchTitle,
                Status = SearchStatus.Value,
            };
            ApiResponse response = await _apiClient.GetTodosAsync(todoQueryObject);
            IsLoading = false;

            if (!response.IsSuccess)
            {
                _eventAggregator.GetEvent<PopupMessageEvent>().Publish("搜索结果异常, 请稍后重试");
                return;
            }

            var todoItems = JsonSerializer.Deserialize<List<TodoItem>>(
                (JsonElement)response.Data,
                JsonHelper.Options
            );

            TodoItems = new ObservableCollection<TodoItem>(todoItems);
        }

        /// <summary>
        /// 通过API保存待办事项数据
        /// </summary>
        private async void ExecuteSaveCommand()
        {
            IsLoading = true;
            var response = await _apiClient.CreateTodoAsync(TodoForCreateDTO);

            if (!response.IsSuccess)
            {
                _eventAggregator
                    .GetEvent<PopupMessageEvent>()
                    .Publish("创建待办事项失败, 请稍后重试");
                return;
            }

            IsRightDrawerOpen = false;
            _eventAggregator.GetEvent<PopupMessageEvent>().Publish("创建待办事项成功");
            await RefreshTodoItemsAsync();
            IsLoading = false;
        }

        private bool CanExecuteSaveCommand()
        {
            return Validator.TryValidateObject(
                TodoForCreateDTO,
                new ValidationContext(TodoForCreateDTO),
                null
            );
        }

        private DelegateCommand<TodoItem> _deleteCommand;
        public DelegateCommand<TodoItem> DeleteCommand =>
            _deleteCommand
            ?? (_deleteCommand = new DelegateCommand<TodoItem>(ExecuteDeleteCommand));

        private async void ExecuteDeleteCommand(TodoItem todoItem)
        {
            IsLoading = true;
            ApiResponse response = await _apiClient.DeleteTodoAsync(todoItem.Id);
            IsLoading = false;

            if (!response.IsSuccess)
            {
                _eventAggregator
                    .GetEvent<PopupMessageEvent>()
                    .Publish("删除待办事项失败, 请稍后重试");
                return;
            }

            _eventAggregator.GetEvent<PopupMessageEvent>().Publish("删除待办事项成功");
            TodoItems.Remove(todoItem);
        }

        public TodoViewModel(IEventAggregator eventAggregator, ApiClient apiClient)
        {
            _eventAggregator = eventAggregator;
            _apiClient = apiClient;

            TodoStatuses = new List<TodoStatus>()
            {
                new TodoStatus() { Name = "全部", Value = -1 },
                new TodoStatus() { Name = "待办", Value = 0 },
                new TodoStatus() { Name = "已完成", Value = 1 },
            };
        }

        /// <summary>
        /// 通过api更新待办事项列表数据
        /// </summary>
        private async Task RefreshTodoItemsAsync()
        {
            ApiResponse response = await _apiClient.GetTodosAsync();

            if (response.IsSuccess != true)
            {
                _eventAggregator.GetEvent<PopupMessageEvent>().Publish("获取待办事项列表数据异常");
                return;
            }

            var todoItems = JsonSerializer.Deserialize<List<TodoItem>>(
                (JsonElement)response.Data,
                JsonHelper.Options
            );

            TodoItems = new ObservableCollection<TodoItem>(todoItems);
        }
    }
}
