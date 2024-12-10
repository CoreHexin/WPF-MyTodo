using MyTodo.Modules.Settings.ViewModels;
using MyTodo.Modules.Settings.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MyTodo.Modules.Settings
{
    public class SettingsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            containerRegistry.RegisterForNavigation<CustomizationView, CustomizationViewModel>();
            containerRegistry.RegisterForNavigation<SystemSettingsView>();
            containerRegistry.RegisterForNavigation<AboutView>();
        }
    }
}