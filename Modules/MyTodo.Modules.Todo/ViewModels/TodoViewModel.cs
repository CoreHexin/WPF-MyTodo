using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using MyTodo.Core.Api;
using MyTodo.Core.DTOs;
using MyTodo.Core.Events;
using MyTodo.Core.Helpers;
using MyTodo.Core.Models;
using MyTodo.Core.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace MyTodo.Modules.Todo.ViewModels
{
    public class TodoViewModel : BindableBase, INavigationAware
    {
        #region 字段
        private readonly IEventAggregator _eventAggregator;
        private readonly ApiClient _apiClient;
        private readonly IMessageBoxService _messageBoxService;
        #endregion

        #region 属性
        public List<TodoStatus> TodoStatuses { get; set; }

        public bool HasSearchResult => TodoItems.Count > 0;

        private TodoStatus _searchStatus;
        public TodoStatus SearchStatus
        {
            get { return _searchStatus; }
            set { SetProperty(ref _searchStatus, value); }
        }

        private string _searchTitle;
        public string SearchTitle
        {
            get { return _searchTitle; }
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

        private ObservableCollection<TodoItem> _todoItems = new ObservableCollection<TodoItem>();
        public ObservableCollection<TodoItem> TodoItems
        {
            get { return _todoItems; }
            set
            {
                if (SetProperty(ref _todoItems, value))
                {
                    RaisePropertyChanged(nameof(HasSearchResult));
                }
            }
        }

        private DelegateCommand _openRightDrawerCommand;
        public DelegateCommand OpenRightDrawerCommand =>
            _openRightDrawerCommand
            ?? (_openRightDrawerCommand = new DelegateCommand(ExecuteOpenRightDrawerCommand));

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

        private DelegateCommand<TodoItem> _deleteCommand;
        public DelegateCommand<TodoItem> DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new DelegateCommand<TodoItem>(Delete));

        #endregion

        #region 构造函数
        public TodoViewModel(
            IEventAggregator eventAggregator,
            ApiClient apiClient,
            IMessageBoxService messageBoxService
        )
        {
            _eventAggregator = eventAggregator;
            _apiClient = apiClient;
            _messageBoxService = messageBoxService;

            TodoStatuses = new List<TodoStatus>()
            {
                new TodoStatus() { Name = "全部", Value = -1 },
                new TodoStatus() { Name = "待办", Value = 0 },
                new TodoStatus() { Name = "已完成", Value = 1 },
            };
        }
        #endregion

        #region 方法
        /// <summary>
        /// 查询待办事项
        /// </summary>
        /// <returns></returns>
        private async Task SearchAsync()
        {
            TodoQueryObject todoQueryObject = new TodoQueryObject()
            {
                Title = SearchTitle,
                Status = SearchStatus.Value,
            };

            IsLoading = true;
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

        private async void Delete(TodoItem todoItem)
        {
            MessageBoxResult result = _messageBoxService.Show("确认删除?", "温馨提示");
            if (result != MessageBoxResult.OK)
            {
                return;
            }

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
            await SearchAsync();
            IsLoading = false;
        }

        private async void ExecuteSearchCommand()
        {
            await SearchAsync();
        }

        private bool CanExecuteSaveCommand()
        {
            return Validator.TryValidateObject(
                TodoForCreateDTO,
                new ValidationContext(TodoForCreateDTO),
                null
            );
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            string title = navigationContext.Parameters.GetValue<string>("Title");
            if (title == "已完成")
            {
                SearchStatus = TodoStatuses[2];
            }
            else
            {
                SearchStatus = TodoStatuses[0];
            }
            ExecuteSearchCommand();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        private void ExecuteOpenRightDrawerCommand()
        {
            IsRightDrawerOpen = true;
        }
        #endregion
    }
}
