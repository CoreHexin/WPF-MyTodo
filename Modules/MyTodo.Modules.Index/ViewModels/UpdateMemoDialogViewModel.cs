using System;
using System.ComponentModel.DataAnnotations;
using MyTodo.Core.Api;
using MyTodo.Core.DTOs;
using MyTodo.Core.Events;
using MyTodo.Core.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MyTodo.Modules.Index.ViewModels
{
    public class UpdateMemoDialogViewModel : BindableBase, IDialogAware
    {
        #region 字段
        private readonly ApiClient _apiClient;
        private readonly IEventAggregator _eventAggregator;
        #endregion

        #region 属性
        public string Title => "添加备忘录";

        public int Id { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private MemoForUpdateDTO _memoForUpdateDTO = new MemoForUpdateDTO();
        public MemoForUpdateDTO MemoForUpdateDTO
        {
            get { return _memoForUpdateDTO; }
            set { SetProperty(ref _memoForUpdateDTO, value); }
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(Cancel));

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand
            ?? (
                _saveCommand = new DelegateCommand(Save, CanExecuteSaveCommand)
                    .ObservesProperty(() => MemoForUpdateDTO.Title)
                    .ObservesProperty(() => MemoForUpdateDTO.Content)
            );
        #endregion

        public event Action<IDialogResult> RequestClose;

        public UpdateMemoDialogViewModel(ApiClient apiClient, IEventAggregator eventAggregator)
        {
            _apiClient = apiClient;
            _eventAggregator = eventAggregator;
        }

        #region 方法
        private async void Save()
        {
            IsLoading = true;
            var response = await _apiClient.UpdateMemoAsync(Id, MemoForUpdateDTO);
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
                MemoForUpdateDTO,
                new ValidationContext(MemoForUpdateDTO),
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

        public void OnDialogOpened(IDialogParameters parameters)
        {
            MemoItem memoItem = parameters.GetValue<MemoItem>("MemoItem");

            Id = memoItem.Id;
            MemoForUpdateDTO.Title = memoItem.Title;
            MemoForUpdateDTO.Content = memoItem.Content;
        }
        #endregion
    }
}
