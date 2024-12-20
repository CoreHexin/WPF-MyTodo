using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using MyTodo.Core;
using MyTodo.Core.Api;
using MyTodo.Core.DTOs;
using MyTodo.Core.Events;
using MyTodo.Core.Helpers;
using MyTodo.Core.Models;
using MyTodo.Modules.Index.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace MyTodo.Modules.Index.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        #region 字段
        private readonly ApiClient _apiClient;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        private readonly IRegionManager _regionManager;
        #endregion

        #region 属性
        private string _welcomeMessage;
        public string WelcomeMessage
        {
            get { return _welcomeMessage; }
            set { SetProperty(ref _welcomeMessage, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private ObservableCollection<StatisticPanel> _statisticPanels;
        public ObservableCollection<StatisticPanel> StatisticPanels
        {
            get { return _statisticPanels; }
            set { SetProperty(ref _statisticPanels, value); }
        }

        private ObservableCollection<TodoItem> _todoItems;
        public ObservableCollection<TodoItem> TodoItems
        {
            get { return _todoItems; }
            set { SetProperty(ref _todoItems, value); }
        }

        private ObservableCollection<MemoItem> _memoItems;
        public ObservableCollection<MemoItem> MemoItems
        {
            get { return _memoItems; }
            set { SetProperty(ref _memoItems, value); }
        }

        private DelegateCommand _showCreateTodoDialogCommand;
        public DelegateCommand ShowCreateTodoDialogCommand =>
            _showCreateTodoDialogCommand
            ?? (_showCreateTodoDialogCommand = new DelegateCommand(ShowCreateTodoDialog));

        private DelegateCommand _showCreateMemoDialogCommand;
        public DelegateCommand ShowCreateMemoDialogCommand =>
            _showCreateMemoDialogCommand
            ?? (_showCreateMemoDialogCommand = new DelegateCommand(ShowCreateMemoDialog));

        private DelegateCommand<TodoItem> _updateTodoStatusCommand;
        public DelegateCommand<TodoItem> UpdateTodoStatusCommand =>
            _updateTodoStatusCommand
            ?? (
                _updateTodoStatusCommand = new DelegateCommand<TodoItem>(
                    ExecuteUpdateTodoStatusCommand
                )
            );

        private DelegateCommand _loadDataCommand;
        public DelegateCommand LoadDataCommand =>
            _loadDataCommand ?? (_loadDataCommand = new DelegateCommand(LoadData));

        private DelegateCommand<TodoItem> _showUpdateTodoDialogCommand;
        public DelegateCommand<TodoItem> ShowUpdateTodoDialogCommand =>
            _showUpdateTodoDialogCommand
            ?? (_showUpdateTodoDialogCommand = new DelegateCommand<TodoItem>(ShowUpdateTodoDialog));

        private DelegateCommand<MemoItem> _showUpdateMemoDialogCommand;
        public DelegateCommand<MemoItem> ShowUpdateMemoDialogCommand =>
            _showUpdateMemoDialogCommand
            ?? (_showUpdateMemoDialogCommand = new DelegateCommand<MemoItem>(ShowUpdateMemoDialog));

        // 统计面板导航命令
        private DelegateCommand<StatisticPanel> _navigateCommand;
        public DelegateCommand<StatisticPanel> NavigateCommand =>
            _navigateCommand ?? (_navigateCommand = new DelegateCommand<StatisticPanel>(Navigate));
        #endregion

        #region 构造函数
        public IndexViewModel(
            ApiClient apiClient,
            IEventAggregator eventAggregator,
            IDialogService dialogService,
            IRegionManager regionManager
        )
        {
            _apiClient = apiClient;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _regionManager = regionManager;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 加载首页数据
        /// </summary>
        private async void LoadData()
        {
            UpdateWelcomeMessage();

            IsLoading = true;
            var statisticTask = CreateStatisticPanelsAsync();
            var todoItemsTask = RefreshTodoItemsAsync();
            var memoItemsTask = RefreshMemoItemsAsync();
            await statisticTask;
            await todoItemsTask;
            await memoItemsTask;
            IsLoading = false;
        }

        /// <summary>
        /// 更新待办事项状态
        /// </summary>
        /// <param name="todoItem"></param>
        private async void ExecuteUpdateTodoStatusCommand(TodoItem todoItem)
        {
            TodoForUpdateDTO todoForUpdateDTO = new TodoForUpdateDTO()
            {
                Title = todoItem.Title,
                Content = todoItem.Content,
                Status = todoItem.Status,
            };

            IsLoading = true;
            ApiResponse response = await _apiClient.UpdateTodoAsync(todoItem.Id, todoForUpdateDTO);

            if (response.IsSuccess != true)
            {
                IsLoading = false;
                _eventAggregator
                    .GetEvent<PopupMessageEvent>()
                    .Publish("待办事项状态更新失败, 请稍后重试");
                return;
            }

            await RefreshTodoItemsAsync();
            await RefreshStatisticPanelsAsync();
            IsLoading = false;
        }

        /// <summary>
        /// 打开添加待办事项对话框
        /// </summary>
        private void ShowCreateTodoDialog()
        {
            _dialogService.ShowDialog(nameof(CreateTodoDialog), CreateTodoDialogCallback);
        }

        /// <summary>
        /// 添加待办事项对话框关闭后的回调方法
        /// </summary>
        /// <param name="dialogResult"></param>
        private async void CreateTodoDialogCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result != ButtonResult.OK)
                return;

            IsLoading = true;
            var todoStatisticTask = RefreshTodoStatisticAsync();
            var todoListTask = RefreshTodoItemsAsync();
            await todoStatisticTask;
            await todoListTask;
            IsLoading = false;
        }

        /// <summary>
        /// 打开添加备忘录对话框
        /// </summary>
        private void ShowCreateMemoDialog()
        {
            _dialogService.ShowDialog(nameof(CreateMemoDialog), CreateMemoDialogCallback);
        }

        /// <summary>
        /// 添加备忘录对话框关闭后的回调方法
        /// </summary>
        /// <param name="dialogResult"></param>
        private async void CreateMemoDialogCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result != ButtonResult.OK)
                return;

            IsLoading = true;
            var memoStatisticTask = RefreshMemoCountAsync();
            var memoListTask = RefreshMemoItemsAsync();
            await memoStatisticTask;
            await memoListTask;
            IsLoading = false;
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

        private async Task RefreshMemoItemsAsync()
        {
            ApiResponse response = await _apiClient.GetMemosAsync();

            if (response.IsSuccess != true)
            {
                _eventAggregator.GetEvent<PopupMessageEvent>().Publish("获取备忘录数据异常");
                return;
            }

            var memoItems = JsonSerializer.Deserialize<List<MemoItem>>(
                (JsonElement)response.Data,
                JsonHelper.Options
            );

            MemoItems = new ObservableCollection<MemoItem>(memoItems);
        }

        private void UpdateWelcomeMessage()
        {
            string[] weeks = new string[]
            {
                "星期日",
                "星期一",
                "星期二",
                "星期三",
                "星期四",
                "星期五",
                "星期六",
            };

            string userName = ((UserApiModel)Application.Current.Properties["User"]).Name;
            WelcomeMessage =
                $"你好, {userName}, 今天是{DateTime.Now.ToString("yyyy-MM-dd")} {weeks[(int)DateTime.Now.DayOfWeek]}";
        }

        /// <summary>
        /// 创建统计面板
        /// </summary>
        private async Task CreateStatisticPanelsAsync()
        {
            StatisticPanels = new ObservableCollection<StatisticPanel>()
            {
                new StatisticPanel()
                {
                    Icon = "ClockFast",
                    Title = "汇总",
                    Content = "",
                    Background = "#FF0CA0FF",
                    Target = "TodoView",
                    Cursor = "Hand",
                },
                new StatisticPanel()
                {
                    Icon = "ClockCheckOutline",
                    Title = "已完成",
                    Content = "",
                    Background = "#FF1ECA3A",
                    Target = "TodoView",
                    Cursor = "Hand",
                },
                new StatisticPanel()
                {
                    Icon = "ChartLineVariant",
                    Title = "完成率",
                    Content = "",
                    Background = "#FF02C6DC",
                    Target = string.Empty,
                    Cursor = null,
                },
                new StatisticPanel()
                {
                    Icon = "PlaylistStar",
                    Title = "备忘录",
                    Content = "",
                    Background = "#FFFFA000",
                    Target = "MemoView",
                    Cursor = "Hand",
                },
            };

            await RefreshStatisticPanelsAsync();
        }

        /// <summary>
        /// 更新统计面板数据
        /// </summary>
        private async Task RefreshStatisticPanelsAsync()
        {
            var todoStatisticTask = RefreshTodoStatisticAsync();
            var memoStatisticTask = RefreshMemoCountAsync();
            await todoStatisticTask;
            await memoStatisticTask;
        }

        /// <summary>
        /// 更新待办事项统计数据
        /// </summary>
        /// <returns></returns>
        private async Task RefreshTodoStatisticAsync()
        {
            var response = await _apiClient.GetTodoStatisticAsync();
            if (response.IsSuccess != true)
            {
                _eventAggregator.GetEvent<PopupMessageEvent>().Publish("获取统计数据异常");
                return;
            }

            var statisticDTO = JsonSerializer.Deserialize<StatisticDTO>(
                (JsonElement)response.Data,
                JsonHelper.Options
            );

            StatisticPanels[0].Content = statisticDTO.TotalCount.ToString();
            StatisticPanels[1].Content = statisticDTO.FinishedCount.ToString();
            StatisticPanels[2].Content = statisticDTO.FinishedRatio;
        }

        /// <summary>
        /// 更新备忘录统计数据
        /// </summary>
        /// <returns></returns>
        private async Task RefreshMemoCountAsync()
        {
            var response = await _apiClient.CountMemoAsync();

            if (response.IsSuccess != true)
            {
                _eventAggregator.GetEvent<PopupMessageEvent>().Publish("获取备忘录统计数据异常");
                return;
            }

            int memoCount = ((JsonElement)response.Data).GetInt32();
            StatisticPanels[3].Content = memoCount.ToString();
        }

        private void Navigate(StatisticPanel statisticPanel)
        {
            if (statisticPanel.Target == string.Empty)
            {
                return;
            }

            NavigationParameters navigationParameter = new NavigationParameters
            {
                { "Title", statisticPanel.Title },
            };

            _regionManager
                .Regions[RegionNames.ContentRegion]
                .RequestNavigate(statisticPanel.Target, navigationParameter);
        }

        private void ShowUpdateMemoDialog(MemoItem memoItem)
        {
            if (memoItem == null)
            {
                return;
            }

            DialogParameters parameters = new DialogParameters() { { "MemoItem", memoItem } };

            _dialogService.ShowDialog(
                nameof(UpdateMemoDialog),
                parameters,
                UpdateMemoDialogCallback
            );
        }

        private async void UpdateMemoDialogCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result != ButtonResult.OK)
            {
                return;
            }

            IsLoading = true;
            var memoStatisticTask = RefreshMemoCountAsync();
            var memoListTask = RefreshMemoItemsAsync();
            await memoStatisticTask;
            await memoListTask;
            IsLoading = false;
        }

        /// <summary>
        /// 弹出修改TodoItem对话框
        /// </summary>
        /// <param name="todoItem"></param>
        public void ShowUpdateTodoDialog(TodoItem todoItem)
        {
            if (todoItem == null)
            {
                return;
            }

            DialogParameters dialogParameters = new DialogParameters();
            dialogParameters.Add("TodoItem", todoItem);

            _dialogService.ShowDialog(
                nameof(UpdateTodoDialog),
                dialogParameters,
                UpdateTodoDialogCallback
            );
        }

        private async void UpdateTodoDialogCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result != ButtonResult.OK)
            {
                return;
            }

            IsLoading = true;
            var todoStatisticTask = RefreshTodoStatisticAsync();
            var todoListTask = RefreshTodoItemsAsync();
            await todoStatisticTask;
            await todoListTask;
            IsLoading = false;
        }

        #endregion
    }
}
