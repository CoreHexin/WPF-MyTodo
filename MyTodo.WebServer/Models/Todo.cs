using System.ComponentModel.DataAnnotations;

namespace MyTodo.WebServer.Models
{
    public class Todo
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        /// <summary>
        /// 状态 0-待办 1-完成
        /// </summary>
        public int Status { get; set; } = 0;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}
