namespace MyTodo.WebServer.DTOs.Todo
{
    public record TodoItemDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int Status { get; set; }
    }
}
