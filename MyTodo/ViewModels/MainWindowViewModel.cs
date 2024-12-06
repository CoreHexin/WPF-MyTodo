using System.Collections.Generic;
using MyTodo.Core.Models;
using Prism.Mvvm;

namespace MyTodo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public List<LeftMenuItem> LeftMenuItems { get; set; }

        private LeftMenuItem _selectedMenu;
        public LeftMenuItem SelectedMenu
        {
            get { return _selectedMenu; }
            set { SetProperty(ref _selectedMenu, value); }
        }

        public MainWindowViewModel()
        {
            InitLeftMenu();
        }

        /// <summary>
        /// 初始化左侧菜单栏
        /// </summary>
        private void InitLeftMenu()
        {
            LeftMenuItems = new List<LeftMenuItem>()
            {
                new LeftMenuItem()
                {
                    Icon = "Home",
                    Name = "首页",
                    ViewName = "IndexView",
                },
                new LeftMenuItem()
                {
                    Icon = "NotebookOutline",
                    Name = "待办事项",
                    ViewName = "TodoView",
                },
                new LeftMenuItem()
                {
                    Icon = "NotebookEditOutline",
                    Name = "备忘录",
                    ViewName = "MemoView",
                },
                new LeftMenuItem()
                {
                    Icon = "Cog",
                    Name = "设置",
                    ViewName = "SettingsView",
                },
            };
        }
    }
}
