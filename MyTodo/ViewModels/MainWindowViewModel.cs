﻿using System.Collections.Generic;
using MyTodo.Core;
using MyTodo.Core.Events;
using MyTodo.Core.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace MyTodo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private IRegionNavigationJournal _regionNavigationJournal;

        public string Title => "My To Do";

        public List<LeftMenuItem> LeftMenuItems { get; set; }

        private LeftMenuItem _selectedMenu;
        public LeftMenuItem SelectedMenu
        {
            get { return _selectedMenu; }
            set { SetProperty(ref _selectedMenu, value); }
        }

        private DelegateCommand _navigateCommand;
        public DelegateCommand NavigateCommand =>
            _navigateCommand ?? (_navigateCommand = new DelegateCommand(Navigate));

        private DelegateCommand _movePrevCommand;
        public DelegateCommand MovePrevCommand =>
            _movePrevCommand ?? (_movePrevCommand = new DelegateCommand(ExecuteMovePrevCommand));

        private DelegateCommand _moveNextCommand;
        public DelegateCommand MoveNextCommand =>
            _moveNextCommand ?? (_moveNextCommand = new DelegateCommand(ExecuteMoveNextCommand));

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            InitLeftMenu();
            _regionManager = regionManager;
            eventAggregator.GetEvent<LeftMenuChangedEvent>().Subscribe(ChangeLeftMenu);
        }

        private void ChangeLeftMenu(string target)
        {
            if (target == SelectedMenu.Target)
            {
                return;
            }

            switch (target)
            {
                case "IndexView":
                    SelectedMenu = LeftMenuItems[0];
                    break;
                case "TodoView":
                    SelectedMenu = LeftMenuItems[1];
                    break;
                case "MemoView":
                    SelectedMenu = LeftMenuItems[2];
                    break;
                default:
                    SelectedMenu = LeftMenuItems[3];
                    break;
            }
        }

        #region 方法

        /// <summary>
        /// 设置默认页面
        /// </summary>
        public void InitDefaultPage()
        {
            SelectedMenu = LeftMenuItems[0];
            Navigate();
        }

        /// <summary>
        /// 导航
        /// </summary>
        private void Navigate()
        {
            _regionManager
                .Regions[RegionNames.ContentRegion]
                .RequestNavigate(
                    SelectedMenu.Target,
                    result =>
                    {
                        _regionNavigationJournal = result.Context.NavigationService.Journal;
                    }
                );
        }

        /// <summary>
        /// 导航前进
        /// </summary>
        private void ExecuteMoveNextCommand()
        {
            if (_regionNavigationJournal != null && _regionNavigationJournal.CanGoForward)
            {
                _regionNavigationJournal.GoForward();
            }
        }

        /// <summary>
        /// 导航后退
        /// </summary>
        private void ExecuteMovePrevCommand()
        {
            if (_regionNavigationJournal != null && _regionNavigationJournal.CanGoBack)
            {
                _regionNavigationJournal.GoBack();
            }
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
                    Target = "IndexView",
                },
                new LeftMenuItem()
                {
                    Icon = "NotebookOutline",
                    Name = "待办事项",
                    Target = "TodoView",
                },
                new LeftMenuItem()
                {
                    Icon = "NotebookEditOutline",
                    Name = "备忘录",
                    Target = "MemoView",
                },
                new LeftMenuItem()
                {
                    Icon = "Cog",
                    Name = "设置",
                    Target = "SettingsView",
                },
            };
        }

        #endregion
    }
}
