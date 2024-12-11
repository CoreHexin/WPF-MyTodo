namespace MyTodo.Core.DTOs
{
    /// <summary>
    /// 待办事项统计
    /// </summary>
    public class StatisticDTO
    {
        public int TotalCount { get; set; }
        public int FinishedCount { get; set; }
        public string FinishedRatio { get; set; }
    }
}
