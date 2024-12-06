using MyTodo.Modules.Todo.ViewModels;
using MyTodo.Modules.Todo.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MyTodo.Modules.Todo
{
    public class TodoModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TodoView, TodoViewModel>();
        }
    }
}