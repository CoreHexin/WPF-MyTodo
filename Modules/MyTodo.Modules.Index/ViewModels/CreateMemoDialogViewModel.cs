using System;
using System.ComponentModel.DataAnnotations;
using MyTodo.Core.Api;
using MyTodo.Core.DTOs;
using MyTodo.Core.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyTodo.Modules.Index.ViewModels
{
    public class CreateMemoDialogViewModel : BindableBase, IDialogAware
    {
        #region 字段
        private readonly ApiClient _apiClient;
        private readonly IEventAggregator _eventAggregator;
        #endregion

        #region 属性
        public string Title => "添加待办事项";

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private MemoForCreateDTO _memoForCreateDTO = new MemoForCreateDTO();
        public MemoForCreateDTO MemoForCreateDTO
        {
            get { return _memoForCreateDTO; }
            set { SetProperty(ref _memoForCreateDTO, value); }
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(Cancel));

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand
            ?? (
                _saveCommand = new DelegateCommand(Save, CanExecuteSaveCommand)
                    .ObservesProperty(() => MemoForCreateDTO.Title)
                    .ObservesProperty(() => MemoForCreateDTO.Content)
            );
        #endregion

        #region 事件
        public event Action<IDialogResult> RequestClose;
        #endregion

        #region 构造函数
        public CreateMemoDialogViewModel(ApiClient apiClient, IEventAggregator eventAggregator)
        {
            _apiClient = apiClient;
            _eventAggregator = eventAggregator;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 保存备忘录
        /// </summary>
        private async void Save()
        {
            IsLoading = true;
            var response = await _apiClient.CreateMemoAsync(MemoForCreateDTO);
            IsLoading = false;

            if (!response.IsSuccess)
            {
                _eventAggregator
                    .GetEvent<PopupMessageEvent>()
                    .Publish("创建备忘录失败, 请稍后重试");
                return;
            }

            _eventAggregator.GetEvent<PopupMessageEvent>().Publish("创建成功");
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        private bool CanExecuteSaveCommand()
        {
            return Validator.TryValidateObject(
                MemoForCreateDTO,
                new ValidationContext(MemoForCreateDTO),
                null
            );
        }

        private void Cancel()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters) { }

        #endregion
    }
}
