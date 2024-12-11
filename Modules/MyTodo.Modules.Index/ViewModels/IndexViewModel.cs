using System;
using System.Collections.ObjectModel;
using System.Windows;
using MyTodo.Core.Models;
using Prism.Mvvm;

namespace MyTodo.Modules.Index.ViewModels
{
    public class IndexViewModel : BindableBase
    {
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

        public IndexViewModel()
        {
            CreateStatisticPanels();
            CreateTestData();

            UpdateWelcomeMessage();
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

            var p = Application.Current.Properties;
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

        private void CreateStatisticPanels()
        {
            StatisticPanels = new ObservableCollection<StatisticPanel>()
            {
                new StatisticPanel()
                {
                    Icon = "ClockFast",
                    Title = "汇总",
                    Content = "10",
                    Background = "#FF0CA0FF",
                    Target = "",
                },
                new StatisticPanel()
                {
                    Icon = "ClockCheckOutline",
                    Title = "已完成",
                    Content = "10",
                    Background = "#FF1ECA3A",
                    Target = "",
                },
                new StatisticPanel()
                {
                    Icon = "ChartLineVariant",
                    Title = "完成率",
                    Content = "80%",
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
        }
    }
}
