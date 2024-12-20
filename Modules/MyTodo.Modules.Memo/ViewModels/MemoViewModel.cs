using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using MyTodo.Core.Api;
using MyTodo.Core.DTOs;
using MyTodo.Core.Events;
using MyTodo.Core.Helpers;
using MyTodo.Core.Models;
using MyTodo.Core.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace MyTodo.Modules.Memo.ViewModels
{
    public class MemoViewModel : BindableBase
    {
        #region 字段

        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageBoxService _messageBoxService;
        private readonly ApiClient _apiClient;

        #endregion

        #region 属性
        public bool HasSearchResult => MemoItems.Count > 0;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private bool _isRightDrawerOpen;
        public bool IsRightDrawerOpen
        {
            get { return _isRightDrawerOpen; }
            set { SetProperty(ref _isRightDrawerOpen, value); }
        }

        private string _searchTitle;
        public string SearchTitle
        {
            get { return _searchTitle; }
            set
            {
                if (SetProperty(ref _searchTitle, value))
                {
                    ExecuteSearchCommand();
                }
            }
        }

        private MemoForCreateDTO _memoForCreateDTO = new MemoForCreateDTO();
        public MemoForCreateDTO MemoForCreateDTO
        {
            get { return _memoForCreateDTO; }
            set { SetProperty(ref _memoForCreateDTO, value); }
        }

        private ObservableCollection<MemoItem> _MemoItems = new ObservableCollection<MemoItem>();
        public ObservableCollection<MemoItem> MemoItems
        {
            get { return _MemoItems; }
            set
            {
                if (SetProperty(ref _MemoItems, value))
                {
                    RaisePropertyChanged(nameof(HasSearchResult));
                }
            }
        }

        private DelegateCommand _openRightDrawerCommand;
        public DelegateCommand OpenRightDrawerCommand =>
            _openRightDrawerCommand
            ?? (_openRightDrawerCommand = new DelegateCommand(OpenRightDrawer));

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand
            ?? (
                _saveCommand = new DelegateCommand(Save, CanExecuteSave)
                    .ObservesProperty(() => MemoForCreateDTO.Title)
                    .ObservesProperty(() => MemoForCreateDTO.Content)
            );

        private DelegateCommand<MemoItem> _deleteCommand;
        public DelegateCommand<MemoItem> DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new DelegateCommand<MemoItem>(Delete));

        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand =>
            _searchCommand ?? (_searchCommand = new DelegateCommand(ExecuteSearchCommand));

        #endregion

        #region 构造函数
        public MemoViewModel(
            ApiClient apiClient,
            IEventAggregator eventAggregator,
            IMessageBoxService messageBoxService
        )
        {
            _apiClient = apiClient;
            _eventAggregator = eventAggregator;
            _messageBoxService = messageBoxService;
        }
        #endregion

        #region 方法
        private async void Delete(MemoItem item)
        {
            MessageBoxResult result = _messageBoxService.Show("确认删除?", "温馨提示");
            if (result != MessageBoxResult.OK)
            {
                return;
            }

            IsLoading = true;
            var response = await _apiClient.DeleteMemoAsync(item.Id);
            IsLoading = false;

            if (!response.IsSuccess)
            {
                _eventAggregator.GetEvent<PopupMessageEvent>().Publish("删除失败, 请稍后重试");
                return;
            }

            _eventAggregator.GetEvent<PopupMessageEvent>().Publish("删除成功");
            MemoItems.Remove(item);
        }

        private async void ExecuteSearchCommand()
        {
            IsLoading = true;
            await SearchAsync();
            IsLoading = false;
        }

        private async Task SearchAsync()
        {
            var response = await _apiClient.GetMemosAsync(SearchTitle);

            if (!response.IsSuccess)
            {
                _eventAggregator.GetEvent<PopupMessageEvent>().Publish("搜索结果异常, 请稍后重试");
                return;
            }

            var memoItems = JsonSerializer.Deserialize<List<MemoItem>>(
                (JsonElement)response.Data,
                JsonHelper.Options
            );

            MemoItems = new ObservableCollection<MemoItem>(memoItems);
        }

        private async void Save()
        {
            IsLoading = true;
            var response = await _apiClient.CreateMemoAsync(MemoForCreateDTO);

            if (!response.IsSuccess)
            {
                _eventAggregator
                    .GetEvent<PopupMessageEvent>()
                    .Publish("创建备忘录失败, 请稍后重试");
                return;
            }

            IsRightDrawerOpen = false;
            _eventAggregator.GetEvent<PopupMessageEvent>().Publish("创建成功");
            await SearchAsync(); // 刷新备忘录列表
            IsLoading = false;
        }

        private bool CanExecuteSave()
        {
            return Validator.TryValidateObject(
                MemoForCreateDTO,
                new ValidationContext(MemoForCreateDTO),
                null
            );
        }

        private void OpenRightDrawer()
        {
            IsRightDrawerOpen = true;
        }
        #endregion
    }
}
