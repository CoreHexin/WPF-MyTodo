namespace MyTodo.WebServer.DTOs.Todo
{
    public record TodoItemStatusUpdateDTO
    {
        public int Id { get; set; }

        public int Status { get; set; }
    }
}
