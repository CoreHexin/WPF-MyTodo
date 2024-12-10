using System.Collections.ObjectModel;
using MyTodo.Core.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace MyTodo.Modules.Todo.ViewModels
{
    public class TodoViewModel : BindableBase
    {
        private bool _isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return _isRightDrawerOpen; }
            set { SetProperty(ref _isRightDrawerOpen, value); }
        }

        private ObservableCollection<TodoItem> _todoItems;
        public ObservableCollection<TodoItem> TodoItems
        {
            get { return _todoItems; }
            set { SetProperty(ref _todoItems, value); }
        }

        private DelegateCommand _openRightDrawerCommand;
        public DelegateCommand OpenRightDrawerCommand =>
            _openRightDrawerCommand
            ?? (_openRightDrawerCommand = new DelegateCommand(ExecuteOpenRightDrawerCommand));

        public TodoViewModel()
        {
            CreateTodoItems();
        }

        private void ExecuteOpenRightDrawerCommand()
        {
            IsRightDrawerOpen = true;
        }

        private void CreateTodoItems()
        {
            TodoItems = new ObservableCollection<TodoItem>();

            for (int i = 0; i < 20; i++)
            {
                TodoItems.Add(
                    new TodoItem()
                    {
                        Id = i,
                        Title = $"待办事项标题{i}",
                        Content = $"待办事项内容{i}",
                    }
                );
            }
        }
    }
}
