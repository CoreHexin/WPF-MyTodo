using System;
using System.Windows;
using MyTodo.Modules.Login;
using MyTodo.Modules.Login.Views;
using MyTodo.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services.Dialogs;

namespace MyTodo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<LoginModule>();
        }

        protected override void OnInitialized()
        {
            var dialogService = Container.Resolve<IDialogService>();
            dialogService.ShowDialog(nameof(LoginView), LoginCallback);
        }

        private void LoginCallback(IDialogResult result)
        {
            if (result.Result != ButtonResult.OK)
            {
                Environment.Exit(0);
                return;
            }
            base.OnInitialized();
        }
    }
}
