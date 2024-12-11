using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using MyTodo.Core.Api;
using MyTodo.Core.DTOs;
using MyTodo.Core.Events;
using MyTodo.Core.Helpers;
using MyTodo.Core.Models;
using Prism.Events;
using Prism.Mvvm;

namespace MyTodo.Modules.Index.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        private readonly ApiClient _apiClient;
        private readonly IEventAggregator _eventAggregator;

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

        public IndexViewModel(ApiClient apiClient, IEventAggregator eventAggregator)
        {
            _apiClient = apiClient;
            _eventAggregator = eventAggregator;

            UpdateWelcomeMessage();
            CreateStatisticPanels();
            CreateTestData();
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

        private void CreateTestData()
        {
            TodoItems = new ObservableCollection<TodoItem>();
            MemoItems = new ObservableCollection<MemoItem>();

            for (int i = 0; i < 10; i++)
            {
                TodoItems.Add(
                    new TodoItem()
                    {
                        Id = i,
                        Title = $"待办事项标题{i}",
                        Content = $"待办内容{i}",
                    }
                );

                MemoItems.Add(
                    new MemoItem()
                    {
                        Id = i,
                        Title = $"备忘录标题{i}",
                        Content = $"备忘录内容{i}",
                    }
                );
            }
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
        /// 更新统计面板数据
        /// </summary>
        private async void RefreshStatisticPanels()
        {
            ApiResponse response = await _apiClient.GetTodoStatistic();

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
