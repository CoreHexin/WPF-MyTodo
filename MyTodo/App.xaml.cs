﻿using MyTodo.Core.Api;
using MyTodo.Core.Models;
using MyTodo.Core.Services;
using MyTodo.Modules.Index;
using MyTodo.Modules.Login;
using MyTodo.Modules.Login.Views;
using MyTodo.Modules.Memo;
using MyTodo.Modules.Settings;
using MyTodo.Modules.Todo;
using MyTodo.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services.Dialogs;
using System;
using System.Windows;

namespace MyTodo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ApiClient>();
            containerRegistry.RegisterSingleton<IMessageBoxService, MessageBoxService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // 登录模块
            moduleCatalog.AddModule<LoginModule>();

            // 首页模块
            moduleCatalog.AddModule<IndexModule>();

            // 待办事项模块
            moduleCatalog.AddModule<TodoModule>();

            // 备忘录模块
            moduleCatalog.AddModule<MemoModule>();

            // 设置模块
            moduleCatalog.AddModule<SettingsModule>();
        }

        protected override void OnInitialized()
        {
            var dialogService = Container.Resolve<IDialogService>();
            dialogService.ShowDialog(nameof(LoginView), LoginCallback);
        }

        private void LoginCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result != ButtonResult.OK)
            {
                Environment.Exit(0);
                return;
            }

            if (dialogResult.Parameters.ContainsKey("User"))
            {
                UserApiModel user = dialogResult.Parameters.GetValue<UserApiModel>("User");
                Current.Properties.Add("User", user);
            }

            base.OnInitialized();
        }
    }
}
