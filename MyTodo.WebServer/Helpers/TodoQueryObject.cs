namespace MyTodo.WebServer.Helpers
{
    public class TodoQueryObject
    {
        public string? Title { get; set; }

        public int Status { get; set; } = -1;
    }
}
