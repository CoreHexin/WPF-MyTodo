using MyTodo.Modules.Login.ViewModels;
using MyTodo.Modules.Login.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MyTodo.Modules.Login
{
    public class LoginModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();
        }
    }
}