namespace MyTodo.WebServer.DTOs.Todo
{
    public class StatisticDTO
    {
        public int TotalCount { get; set; }
        public int FinishedCount { get; set; }
        public string FinishedRatio =>
            TotalCount == 0 ? "0" : (FinishedCount * 100.0 / TotalCount).ToString("f2") + "%";
    }
}
