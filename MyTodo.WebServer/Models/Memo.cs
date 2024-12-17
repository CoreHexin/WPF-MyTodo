using System.ComponentModel.DataAnnotations;

namespace MyTodo.WebServer.Models
{
    public class Memo
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
