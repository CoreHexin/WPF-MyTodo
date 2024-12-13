namespace MyTodo.WebServer.DTOs.Todo
{
    public record TodoStatusUpdateDTO
    {
        public int Id { get; set; }

        public int Status { get; set; }
    }
}
