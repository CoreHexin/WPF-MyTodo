namespace MyTodo.Core.Helpers
{
    public class TodoQueryObject
    {
        public string? Title { get; set; }

        public int Status { get; set; } = -1;
    }
}
