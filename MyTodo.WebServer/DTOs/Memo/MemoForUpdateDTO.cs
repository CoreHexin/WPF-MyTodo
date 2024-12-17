namespace MyTodo.WebServer.DTOs.Memo
{
    public record MemoForUpdateDTO
    {
        public string Title { get; set; }

        public string Content { get; set; }
    }
}
