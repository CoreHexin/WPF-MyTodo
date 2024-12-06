using MyTodo.Modules.Memo.ViewModels;
using MyTodo.Modules.Memo.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MyTodo.Modules.Memo
{
    public class MemoModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MemoView, MemoViewModel>();
        }
    }
}