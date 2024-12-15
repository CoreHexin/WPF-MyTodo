namespace MyTodo.WebServer.DTOs.Todo
{
    public record TodoForUpdateDTO
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int Status { get; set; }
    }
}
