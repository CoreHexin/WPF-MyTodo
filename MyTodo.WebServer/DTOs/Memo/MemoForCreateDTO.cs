namespace MyTodo.WebServer.DTOs.Memo
{
    public record MemoForCreateDTO
    {
        public string Title { get; set; }

        public string Content { get; set; }
    }
}
