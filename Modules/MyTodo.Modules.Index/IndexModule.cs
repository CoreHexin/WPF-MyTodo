using MyTodo.Modules.Index.ViewModels;
using MyTodo.Modules.Index.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MyTodo.Modules.Index
{
    public class IndexModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();

            containerRegistry.RegisterDialog<CreateTodoDialog, CreateTodoDialogViewModel>();
            containerRegistry.RegisterDialog<UpdateTodoDialog, UpdateTodoDialogViewModel>();

            containerRegistry.RegisterDialog<CreateMemoDialog, CreateMemoDialogViewModel>();
        }
    }
}