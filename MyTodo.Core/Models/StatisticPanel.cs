using Prism.Mvvm;

namespace MyTodo.Core.Models
{
    public class StatisticPanel : BindableBase
    {
        public string Icon { get; set; }

        public string Title { get; set; }

        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        public string Background { get; set; }

        public string Target { get; set; }
    }
}
