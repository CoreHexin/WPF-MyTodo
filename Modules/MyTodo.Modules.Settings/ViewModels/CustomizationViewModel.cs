using System;
using MaterialDesignThemes.Wpf;
using Prism.Mvvm;

namespace MyTodo.Modules.Settings.ViewModels
{
    public class CustomizationViewModel : BindableBase
    {
        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    ModifyTheme(theme =>
                        theme.SetBaseTheme(value ? BaseTheme.Dark : BaseTheme.Light)
                    );
                }
            }
        }

        public CustomizationViewModel()
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();

            IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark;
        }

        private static void ModifyTheme(Action<Theme> modificationAction)
        {
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }
    }
}
