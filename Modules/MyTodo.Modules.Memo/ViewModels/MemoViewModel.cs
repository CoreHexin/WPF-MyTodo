using MyTodo.Core.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MyTodo.Modules.Memo.ViewModels
{
    public class MemoViewModel : BindableBase
    {
        private bool _isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return _isRightDrawerOpen; }
            set { SetProperty(ref _isRightDrawerOpen, value); }
        }

        private ObservableCollection<MemoItem> _MemoItems;
        public ObservableCollection<MemoItem> MemoItems
        {
            get { return _MemoItems; }
            set { SetProperty(ref _MemoItems, value); }
        }

        private DelegateCommand _openRightDrawerCommand;
        public DelegateCommand OpenRightDrawerCommand =>
            _openRightDrawerCommand
            ?? (_openRightDrawerCommand = new DelegateCommand(ExecuteOpenRightDrawerCommand));

        public MemoViewModel()
        {
            CreateMemoItems();
        }

        private void ExecuteOpenRightDrawerCommand()
        {
            IsRightDrawerOpen = true;
        }

        private void CreateMemoItems()
        {
            MemoItems = new ObservableCollection<MemoItem>();

            for (int i = 0; i < 20; i++)
            {
                MemoItems.Add(
                    new MemoItem()
                    {
                        Id = i,
                        Title = $"备忘录标题{i}",
                        Content = $"备忘录内容{i}",
                    }
                );
            }
        }
    }
}

