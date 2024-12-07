using MyTodo.Core.Models;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MyTodo.Modules.Index.ViewModels
{
    public class IndexViewModel : BindableBase
    {
        private ObservableCollection<StatisticPanel> _statisticPanels;
        public ObservableCollection<StatisticPanel> StatisticPanels
        {
            get { return _statisticPanels; }
            set { SetProperty(ref _statisticPanels, value); }
        }

        public IndexViewModel()
        {
            CreateStatisticPanels();
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
