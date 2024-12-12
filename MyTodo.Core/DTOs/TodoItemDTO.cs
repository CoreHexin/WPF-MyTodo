using MyTodo.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace MyTodo.Core.DTOs
{
    public class TodoItemDTO : NotifyDataErrorInfo
    {
        private string _title = string.Empty;

        [Required(ErrorMessage = "标题不能为空")]
        public string Title
        {
            get { return _title; }
            set 
            { 
                if (SetProperty(ref _title, value))
                {
                    Validate(nameof(Title), value);
                }
            }
        }

        private string _content = string.Empty;

        [Required(ErrorMessage = "内容不能为空")]
        public string Content
        {
            get { return _content; }
            set
            {
                if (SetProperty(ref _content, value))
                {
                    Validate(nameof(Content), value);
                }
            }
        }

        public int Status { get; set; }
    }
}
