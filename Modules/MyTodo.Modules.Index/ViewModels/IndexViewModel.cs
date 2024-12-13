using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using MyTodo.Core.Api;
using MyTodo.Core.DTOs;
using MyTodo.Core.Events;
using MyTodo.Core.Helpers;
using MyTodo.Core.Models;
using MyTodo.Modules.Index.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyTodo.Modules.Index.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        private readonly ApiClient _apiClient;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;

        private string _welcomeMessage;
        public string WelcomeMessage
        {
            get { return _welcomeMessage; }
            set { SetProperty(ref _welcomeMessage, value); }
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

        private DelegateCommand _openAddTodoDialogCommand;
        public DelegateCommand OpenAddTodoDialogCommand =>
            _openAddTodoDialogCommand
            ?? (_openAddTodoDialogCommand = new DelegateCommand(ExecuteOpenAddTodoDialogCommand));

        private DelegateCommand<TodoItem> _updateTodoStatusCommand;
        public DelegateCommand<TodoItem> UpdateTodoStatusCommand =>
            _updateTodoStatusCommand
            ?? (
                _updateTodoStatusCommand = new DelegateCommand<TodoItem>(
                    ExecuteUpdateTodoStatusCommand
                )
            );

        public IndexViewModel(
            ApiClient apiClient,
            IEventAggregator eventAggregator,
            IDialogService dialogService
        )
        {
            _apiClient = apiClient;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;

            UpdateWelcomeMessage();
            CreateStatisticPanels();
            RefreshTodoItems();
        }

        /// <summary>
        /// 更新待办事项状态
        /// </summary>
        /// <param name="todoItem"></param>
        private async void ExecuteUpdateTodoStatusCommand(TodoItem todoItem)
        {
            ApiResponse response = await _apiClient.UpdateTodoStatusAsync(todoItem);

            if (response.IsSuccess != true)
            {
                _eventAggregator
                    .GetEvent<PopupMessageEvent>()
                    .Publish("待办事项状态更新失败, 请稍后重试");
                return;
            }

            RefreshTodoItems();
            RefreshStatisticPanels();
        }

        /// <summary>
        /// 打开添加待办事项对话框
        /// </summary>
        private void ExecuteOpenAddTodoDialogCommand()
        {
            _dialogService.ShowDialog(nameof(AddTodoDialog), AddTodoDialogCallback);
        }

        /// <summary>
        /// 待办事项对话框关闭后的回调方法
        /// </summary>
        /// <param name="dialogResult"></param>
        private void AddTodoDialogCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result != ButtonResult.OK)
                return;

            // 更新统计面板数据
            RefreshStatisticPanels();

            // 更新待办事项数据
            RefreshTodoItems();
        }

        /// <summary>
        /// 通过api更新待办事项列表数据
        /// </summary>
        private async void RefreshTodoItems()
        {
            ApiResponse response = await _apiClient.GetTodoItemsAsync();

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
        private void CreateStatisticPanels()
        {
            StatisticPanels = new ObservableCollection<StatisticPanel>()
            {
                new StatisticPanel()
                {
                    Icon = "ClockFast",
                    Title = "汇总",
                    Content = "",
                    Background = "#FF0CA0FF",
                    Target = "",
                },
                new StatisticPanel()
                {
                    Icon = "ClockCheckOutline",
                    Title = "已完成",
                    Content = "",
                    Background = "#FF1ECA3A",
                    Target = "",
                },
                new StatisticPanel()
                {
                    Icon = "ChartLineVariant",
                    Title = "完成率",
                    Content = "",
                    Background = "#FF02C6DC",
                    Target = "",
                },
                new StatisticPanel()
                {
                    Icon = "PlaylistStar",
                    Title = "备忘录",
                    Content = "8",
                    Background = "#FFFFA000",
                    Target = "",
                },
            };

            RefreshStatisticPanels();
        }

        /// <summary>
        /// 通过api更新统计面板数据
        /// </summary>
        private async void RefreshStatisticPanels()
        {
            ApiResponse response = await _apiClient.GetTodoStatisticAsync();

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
    }
}
