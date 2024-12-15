namespace MyTodo.WebServer.DTOs.Todo
{
    public record TodoForCreateDTO
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int Status { get; set; }
    }
}
