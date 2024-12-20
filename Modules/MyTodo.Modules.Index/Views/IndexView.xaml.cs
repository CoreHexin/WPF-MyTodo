using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyTodo.Modules.Index.Views
{
    /// <summary>
    /// Interaction logic for IndexView
    /// </summary>
    public partial class IndexView : UserControl
    {
        public IndexView()
        {
            InitializeComponent();
        }

        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var listbox = sender as ListBox;
            if (listbox == null)
            {
                return;
            }

            var scrollViewer = FindVisualChild<ScrollViewer>(listbox);
            if (scrollViewer == null)
            {
                return;
            }

            if (e.Delta > 0)
            {
                // 鼠标向上滚动
                scrollViewer.PageUp();
            }
            else if (e.Delta < 0)
            {
                // 鼠标向下滚动
                scrollViewer.PageDown();
            }

            e.Handled = true;
        }

        private T FindVisualChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T correctlyTyped)
                {
                    return correctlyTyped;
                }

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
    }
}
