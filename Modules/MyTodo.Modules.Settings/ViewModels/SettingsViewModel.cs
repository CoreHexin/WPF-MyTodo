using System.Collections.Generic;
using MyTodo.Core;
using MyTodo.Core.Events;
using MyTodo.Core.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace MyTodo.Modules.Settings.ViewModels
{
    public class SettingsViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private LeftMenuItem _selectedMenu;
        public LeftMenuItem SelectedMenu
        {
            get { return _selectedMenu; }
            set { SetProperty(ref _selectedMenu, value); }
        }
        public List<LeftMenuItem> LeftMenuItems { get; set; }

        private DelegateCommand _navigateCommand;

        public DelegateCommand NavigateCommand =>
            _navigateCommand ?? (_navigateCommand = new DelegateCommand(Navigate));

        public SettingsViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            CreateLeftMenuItems();
        }

        private void Navigate()
        {
            _regionManager.Regions[RegionNames.SettingsRegion].RequestNavigate(SelectedMenu.Target);
        }

        private void CreateLeftMenuItems()
        {
            LeftMenuItems = new List<LeftMenuItem>()
            {
                new LeftMenuItem()
                {
                    Icon = "Palette",
                    Name = "个性化",
                    Target = "CustomizationView",
                },
                new LeftMenuItem()
                {
                    Icon = "Cog",
                    Name = "系统设置",
                    Target = "SystemSettingsView",
                },
                new LeftMenuItem()
                {
                    Icon = "Information",
                    Name = "关于",
                    Target = "AboutView",
                },
            };
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<LeftMenuChangedEvent>().Publish("SettingsView");
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
