﻿using MyTodo.Core.Events;
using MyTodo.ViewModels;
using Prism.Events;
using System.Windows;
using System.Windows.Input;

namespace MyTodo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IEventAggregator _eventAggregator;

        public MainWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PopupMessageEvent>().Subscribe(PopupMessage);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 加载默认页面
            ((MainWindowViewModel)DataContext).InitDefaultPage();
        }

        private void ColorZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ColorZone_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        // 收起左侧抽屉菜单栏
        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }
        private void PopupMessage(string msg)
        {
            snackbar.MessageQueue.Enqueue(msg);
        }

    }
}
