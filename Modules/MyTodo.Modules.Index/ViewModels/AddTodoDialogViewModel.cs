using MaterialDesignThemes.Wpf;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace MyTodo.Modules.Index.ViewModels
{
    public class AddTodoDialogViewModel : BindableBase, IDialogAware
    {
        public string Title => "添加待办事项";

        public event Action<IDialogResult> RequestClose;

        public AddTodoDialogViewModel()
        {
            //ModifyTheme();
        }

        private static void ModifyTheme()
        {
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(BaseTheme.Dark);
            paletteHelper.SetTheme(theme);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
